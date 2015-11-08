﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.DeviceSystem;
using Mosa.FileSystem.FAT;
using System;
using System.IO;
using System.Collections.Generic;
using Mosa.ClassLib;

namespace Mosa.Utility.BootImage
{
	/// <summary>
	///
	/// </summary>
	public class Generator
	{
		/// <summary>
		/// Creates the specified options.
		/// </summary>
		/// <param name="options">The options.</param>
		static public void Create(Options options)
		{
			if (File.Exists(options.DiskImageFileName))
			{
				File.Delete(options.DiskImageFileName);
			}

			uint blockCount = options.BlockCount;

			if (blockCount == 0)
			{
				blockCount = 8400 + 1;
				foreach (var file in options.IncludeFiles)
				{
					blockCount += ((uint)file.Content.Length / 512) + 1;
				}
			}

			var diskGeometry = new DiskGeometry();
			diskGeometry.GuessGeometry(blockCount);

			// Create disk image file
			BlockFileStream diskDevice = new BlockFileStream(options.DiskImageFileName);

			if (options.ImageFormat == ImageFormatType.VDI)
			{
				// Create header
				byte[] header = VDI.CreateHeader(
					blockCount,
					options.MediaGuid.ToByteArray(),
					options.MediaLastSnapGuid.ToByteArray(),
					diskGeometry
				);

				diskDevice.WriteBlock(0, 1, header);

				byte[] map = VDI.CreateImageMap(blockCount);

				diskDevice.WriteBlock(1, (uint)(map.Length / 512), map);

				diskDevice.BlockOffset = 1 + (uint)(map.Length / 512);
			}

			// Expand disk image
			diskDevice.WriteBlock(blockCount - 1, 1, new byte[512]);

			// Create partition device
			PartitionDevice partitionDevice;

			if (options.MBROption)
			{
				// Create master boot block record
				MasterBootBlock mbr = new MasterBootBlock(diskDevice);

				// Setup partition entry
				mbr.DiskSignature = 0x12345678;
				mbr.Partitions[0].Bootable = true;
				mbr.Partitions[0].StartLBA = diskGeometry.SectorsPerTrack;
				mbr.Partitions[0].TotalBlocks = blockCount - mbr.Partitions[0].StartLBA;

				switch (options.FileSystem)
				{
					case FileSystemType.FAT12: mbr.Partitions[0].PartitionType = PartitionType.FAT12; break;
					case FileSystemType.FAT16: mbr.Partitions[0].PartitionType = PartitionType.FAT16; break;
					case FileSystemType.FAT32: mbr.Partitions[0].PartitionType = PartitionType.FAT32; break;
					default: break;
				}

				mbr.Code = options.MBRCode;

				mbr.Write();

				partitionDevice = new PartitionDevice(diskDevice, mbr.Partitions[0], false);
			}
			else
			{
				partitionDevice = new PartitionDevice(diskDevice, false);
			}

			// Set FAT settings
			FatSettings fatSettings = new FatSettings();

			switch (options.FileSystem)
			{
				case FileSystemType.FAT12: fatSettings.FATType = FatType.FAT12; break;
				case FileSystemType.FAT16: fatSettings.FATType = FatType.FAT16; break;
				case FileSystemType.FAT32: fatSettings.FATType = FatType.FAT32; break;
				default: break;
			}

			fatSettings.FloppyMedia = false;
			fatSettings.VolumeLabel = options.VolumeLabel;
			fatSettings.SerialID = new byte[4] { 0x01, 0x02, 0x03, 0x04 };
			fatSettings.SectorsPerTrack = diskGeometry.SectorsPerTrack;
			fatSettings.NumberOfHeads = diskGeometry.Heads;
			fatSettings.HiddenSectors = diskGeometry.SectorsPerTrack;
			fatSettings.OSBootCode = options.FatBootCode;

			// Create FAT file system
			FatFileSystem fat = new FatFileSystem(partitionDevice);
			if (!fat.Format(fatSettings))
			{
				throw new Exception("ERROR: Invalid FAT settings");
			}

			fat.SetVolumeName(options.VolumeLabel);

			foreach (var includeFile in options.IncludeFiles)
			{
				Mosa.FileSystem.FAT.FatFileAttributes fileAttributes = new Mosa.FileSystem.FAT.FatFileAttributes();
				if (includeFile.Archive) fileAttributes |= Mosa.FileSystem.FAT.FatFileAttributes.Archive;
				if (includeFile.ReadOnly) fileAttributes |= Mosa.FileSystem.FAT.FatFileAttributes.ReadOnly;
				if (includeFile.Hidden) fileAttributes |= Mosa.FileSystem.FAT.FatFileAttributes.Hidden;
				if (includeFile.System) fileAttributes |= Mosa.FileSystem.FAT.FatFileAttributes.System;

				string newname = (Path.GetFileNameWithoutExtension(includeFile.Filename).PadRight(8).Substring(0, 8) + Path.GetExtension(includeFile.Filename).PadRight(4).Substring(1, 3)).ToUpper();
				FatFileLocation location = fat.CreateFile(newname, fileAttributes, 0);

				if (!location.IsValid)
					throw new Exception("Unable to write file");

				FatFileStream fatFileStream = new FatFileStream(fat, location);
				fatFileStream.Write(includeFile.Content, 0, includeFile.Content.Length);
				fatFileStream.Flush();
			}

			if (options.PatchSyslinuxOption)
			{
				// Locate ldlinux.sys file for patching
				string filename = "ldlinux.sys";
				string name = (Path.GetFileNameWithoutExtension(filename) + Path.GetExtension(filename).PadRight(4).Substring(0, 4)).ToUpper();

				FatFileLocation location = fat.FindEntry(new Mosa.FileSystem.FAT.Find.WithName(name), 0);

				if (location.IsValid)
				{
					// Read boot sector
					BinaryFormat bootSector = new BinaryFormat(partitionDevice.ReadBlock(0, 1));

					// Get the file size & number of sectors used
					uint fileSize = fat.GetFileSize(location.DirectorySector, location.DirectorySectorIndex);
					uint sectorCount = (fileSize + 511) >> 9;

					List<uint> sectors = new List<uint>();

					// Create list of the sectors of the file
					for (uint cluster = location.FirstCluster; (cluster != 0); cluster = fat.GetNextCluster(cluster))
					{
						uint sec = fat.GetSectorByCluster(cluster);
						for (uint s = 0; s < fat.SectorsPerCluster; s++)
						{
							sectors.Add(sec + s);
						}
					}

					// Read the first cluster of the file
					BinaryFormat firstCluster = new BinaryFormat(fat.ReadCluster(location.FirstCluster));

					uint patchArea = 0;

					// Search for 0x3EB202FE (magic)
					for (patchArea = 0; (firstCluster.GetUInt(patchArea) != 0x3EB202FE) && (patchArea < fat.ClusterSizeInBytes); patchArea += 4) ;

					if (patchArea >= fat.ClusterSizeInBytes)
						throw new InvalidProgramException("Unable to find patch location for syslinux");

					// Set up the totals
					firstCluster.SetUShort(patchArea + Syslinux.PatchAreaOffset.DataSectors, (ushort)(sectorCount));
					firstCluster.SetUShort(patchArea + Syslinux.PatchAreaOffset.AdvSectors, 2);
					firstCluster.SetUInt(patchArea + Syslinux.PatchAreaOffset.Dwords, (fileSize >> 2));

					// Clear sector entries
					firstCluster.Fill(patchArea + 16, 0, (uint)(sectors.Count * 4));

					// Set sector entries
					for (int i = 0; i < sectors.Count; i++)
					{
						firstCluster.SetUInt((uint)(patchArea + 16 + (i * 4)), sectors[i]);
					}

					// Clear out checksum
					firstCluster.SetUInt(patchArea + 12, 0);

					// Write back the updated cluster
					fat.WriteCluster(location.FirstCluster, firstCluster.Data);

					// Re-Calculate checksum by opening the file
					FatFileStream file = new FatFileStream(fat, location);

					uint csum = 0x3EB202FE;
					for (uint index = 0; index < (file.Length >> 2); index++)
					{
						uint value = (uint)file.ReadByte() | ((uint)file.ReadByte() << 8) | ((uint)file.ReadByte() << 16) | ((uint)file.ReadByte() << 24);
						csum -= value;
					}

					// Set the checksum
					firstCluster.SetUInt(patchArea + 12, csum);

					// Write patched cluster back to disk
					fat.WriteCluster(location.FirstCluster, firstCluster.Data);

					// Set the first sector location of the file
					// TBD: 0x1FB may be dynamic
					bootSector.SetUInt(0x1F8, fat.GetSectorByCluster(location.FirstCluster));
					bootSector.SetUInt(0x1F8 + 4, 0);

					// Change jump address
					//bootSector.SetUInt(0x01, 0x58);

					// Write back patched boot sector
					partitionDevice.WriteBlock(0, 1, bootSector.Data);
				}
			}

			if (options.ImageFormat == ImageFormatType.VHD)
			{
				// Create footer
				byte[] footer = VHD.CreateFooter(
					blockCount,
					(uint)(DateTime.Now - (new DateTime(2000, 1, 1, 0, 0, 0))).Seconds,
					options.MediaGuid.ToByteArray(),
					diskGeometry
				);

				diskDevice.WriteBlock(blockCount, 1, footer);
			}

			diskDevice.Dispose();
		}
	}
}
