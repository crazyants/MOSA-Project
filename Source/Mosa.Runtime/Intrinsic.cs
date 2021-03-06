﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Runtime.CompilerServices;

namespace Mosa.Runtime
{
	/// <summary>
	/// Provides stub methods for selected native IR instructions.
	/// </summary>
	public static unsafe class Intrinsic
	{
		#region Intrinsic

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte Load8(uint address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte Load8(uint address, int offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte Load8(uint address, uint offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ushort Load16(uint address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ushort Load16(uint address, int offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ushort Load16(uint address, uint offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint Load32(uint address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint Load32(uint address, int offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint Load32(uint address, uint offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong Load64(uint address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong Load64(uint address, int offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ulong Load64(uint address, uint offset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store8(uint address, byte value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store8(uint address, int offset, byte value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store8(uint address, uint offset, byte value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store16(uint address, ushort value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store16(uint address, int offset, ushort value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store16(uint address, uint offset, ushort value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store32(uint address, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store32(uint address, int offset, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store32(uint address, uint offset, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store64(uint address, ulong value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store64(uint address, int offset, ulong value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Store64(uint address, uint offset, ulong value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void* GetObjectAddress<T>(T obj) where T : class;

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void* GetValueTypeAddress<T>(T obj) where T : struct;

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object GetObjectFromAddress(void* address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern static object CreateInstanceSimple(void* ctor, void* thisObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern static uint* GetAssemblyListTable();

		#endregion Intrinsic
	}
}
