using System;
namespace SupRip
{
	internal class SUPFileFormatException : Exception
	{
		public SUPFileFormatException(string reason) : base(reason)
		{
		}
		public SUPFileFormatException()
		{
		}
	}
}
