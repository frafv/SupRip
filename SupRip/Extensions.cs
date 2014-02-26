using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupRip
{
	public static class Extensions
	{
		public static void ForEach<TItem>(this ParallelQuery<TItem> source, Func<TItem, bool> action)
		{
			var stack = new Dictionary<int, TItem>();
			int pos = 0;
			bool stop = false;
			source.Select((item, i) => new { item, i }).ForAll(data =>
			{
				if (stop) return;
				lock (stack)
				{
					if (stop) return;
					if (data.i == pos)
					{
						stop = !action(data.item);
						pos++;
					}
					else
					{
						stack.Add(data.i, data.item);
					}
					TItem item;
					while (!stop && stack.TryGetValue(pos, out item))
					{
						stop = !action(item);
						pos++;
					}
				}
			});
		}

		public static void RangeForAll(this int count, Action<int> action, bool parallel = true)
		{
			if (parallel)
				ParallelEnumerable.Range(0, count).ForAll(action);
			else
				for (int i = 0; i < count; i++) action(i);
		}

		public static void ForEach<TItem>(this List<TItem> list, Action<TItem> action, bool parallel)
		{
			if (parallel)
				list.AsParallel().ForAll(action);
			else
				list.ForEach(action);
		}

		public static uint LowEndianInt32(this Stream fs)
		{
			byte[] array = new byte[4];
			fs.Read(array, 0, 4);
			return (uint)((int)array[0] + ((int)array[1] << 8) + ((int)array[2] << 16) + ((int)array[3] << 24));
		}

		public static uint BigEndianInt32(this Stream fs)
		{
			byte[] array = new byte[4];
			fs.Read(array, 0, 4);
			return (uint)((int)array[3] + ((int)array[2] << 8) + ((int)array[1] << 16) + ((int)array[0] << 24));
		}

		public static int BigEndianInt16(this Stream fs)
		{
			byte[] array = new byte[2];
			fs.Read(array, 0, 2);
			return (int)array[1] + ((int)array[0] << 8);
		}

		public static int BigEndianInt24(this Stream fs)
		{
			byte[] array = new byte[3];
			fs.Read(array, 0, 3);
			return (int)array[2] + ((int)array[1] << 8) + ((int)array[0] << 16);
		}

		public static int LowEndianInt16(this Stream fs)
		{
			byte[] array = new byte[2];
			fs.Read(array, 0, 2);
			return (int)array[0] + ((int)array[1] << 8);
		}
	}
}
