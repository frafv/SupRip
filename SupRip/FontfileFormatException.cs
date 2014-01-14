using System;
namespace SupRip
{
	internal class FontfileFormatException : Exception
	{
		public FontfileFormatException(string reason) : base(reason)
		{
		}
	}
}
