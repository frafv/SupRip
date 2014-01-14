using System;
namespace SupRip
{
	internal class SSTFileFormatException : Exception
	{
		public SSTFileFormatException(string reason) : base(reason)
		{
		}
	}
}
