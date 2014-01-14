using System;
using System.Drawing;
namespace SupRip
{
	internal class PositionedString
	{
		private Rectangle position;
		private string str;
		public Rectangle Position
		{
			get
			{
				return this.position;
			}
		}
		public string Str
		{
			get
			{
				return this.str;
			}
		}
		public PositionedString(Rectangle p, string s)
		{
			this.position = p;
			this.str = s;
		}
	}
}
