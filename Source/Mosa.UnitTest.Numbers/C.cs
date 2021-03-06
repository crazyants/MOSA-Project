﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Collections.Generic;

namespace Mosa.UnitTest.Numbers
{
	public static class C
	{
		private static IList<char> series = null;

		public static IEnumerable<char> Series
		{
			get
			{
				if (series == null) series = GetSeries();

				foreach (char value in series)
					yield return value;
			}
		}

		public static IList<char> GetSeries()
		{
			List<char> list = new List<char>();

			list.AddIfNew<char>((char)1);
			list.AddIfNew<char>((char)2);
			list.AddIfNew<char>((char)127);
			list.AddIfNew<char>(char.MinValue);
			list.AddIfNew<char>(char.MaxValue);
			list.AddIfNew<char>('0');
			list.AddIfNew<char>('9');
			list.AddIfNew<char>('A');
			list.AddIfNew<char>('Z');
			list.AddIfNew<char>('a');
			list.AddIfNew<char>('z');
			list.AddIfNew<char>(' ');
			list.AddIfNew<char>('\n');
			list.AddIfNew<char>('\t');

			list.Sort();

			return list;
		}
	}
}
