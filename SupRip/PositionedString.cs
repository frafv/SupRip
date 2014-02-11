using System;
using System.Drawing;
namespace SupRip
{
	internal class PositionedString
	{
		public RectangleF Position
		{
			get;
			private set;
		}
		public string Str
		{
			get;
			private set;
		}
		public PositionedString(RectangleF p, string s)
		{
			this.Position = p;
			this.Str = s;
		}
	}
}
