using System;
using System.Drawing;
namespace SupRip
{
	internal class Space
	{
		public enum SpaceType
		{
			Straight,
			TopRight,
			TopLeft,
			BottomRight,
			BottomLeft
		}
		private Rectangle rect;
		private bool partial;
		private Space.SpaceType type;
		private double angle;
		private int slopeStart;
		public Rectangle Rect
		{
			get
			{
				return this.rect;
			}
		}
		public Space.SpaceType Type
		{
			get
			{
				return this.type;
			}
		}
		public double Angle
		{
			get
			{
				return this.angle;
			}
		}
		public int SlopeStart
		{
			get
			{
				return this.slopeStart;
			}
		}
		public bool Partial
		{
			get
			{
				return this.partial;
			}
			set
			{
				this.partial = value;
			}
		}
		public int Hash
		{
			get
			{
				return this.rect.Left * 1000 + this.rect.Top;
			}
		}
		public Space(Rectangle rect) : this(rect, false)
		{
		}
		public Space(Rectangle rect, bool isPartial)
		{
			this.rect = rect;
			this.partial = isPartial;
		}
		public Space(int left, int top, int width, int height, bool isPartial)
		{
			this.rect = new Rectangle(left, top, width, height);
			this.partial = isPartial;
		}
		public Space(int left, int top, int width, int height, bool isPartial, Space.SpaceType t, int slope, double a) : this(left, top, width, height, isPartial)
		{
			this.type = t;
			this.angle = a;
			this.slopeStart = slope;
		}
		public Space(int left, int top, int width, int height) : this(left, top, width, height, false)
		{
		}
		public void Resize(int left, int top, int right, int bottom)
		{
			this.rect.X = this.rect.X - left;
			this.rect.Width = this.rect.Width + (left + right);
			this.rect.Y = this.rect.Y - top;
			this.rect.Height = this.rect.Height + (top + bottom);
		}
		public override string ToString()
		{
			string text = "";
			if (this.partial)
			{
				text = "partial ";
			}
			switch (this.type)
			{
			case Space.SpaceType.Straight:
				return text + "straight";
			case Space.SpaceType.TopRight:
				return text + "topright";
			case Space.SpaceType.TopLeft:
				return text + "topleft";
			case Space.SpaceType.BottomRight:
				return text + "bottomright";
			case Space.SpaceType.BottomLeft:
				return text + "bottomleft";
			default:
				return text;
			}
		}
	}
}
