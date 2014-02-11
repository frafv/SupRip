using System;
using System.Collections.Generic;
using System.Drawing;

namespace SupRip
{
	internal class Space
	{
		public const float smoothStep = 0.5f;

		public Rectangle Rect
		{
			get;
			private set;
		}

		public float[] Left
		{
			get;
			private set;
		}

		public float[] Right
		{
			get;
			private set;
		}

		public int Height
		{
			get { return this.Left.Length; }
		}

		public int Hash
		{
			get
			{
				return this.Rect.Left * 1000 + this.Rect.Top;
			}
		}

		public Space(Rectangle rect)
		{
			this.Rect = rect;
		}

		public Space(int left, int top, int width, int height)
		{
			this.Rect = new Rectangle(left, top, width, height);
		}

		public Space(int height)
		{
			this.Left = new float[height];
			this.Right = new float[height];
		}

		public void SetRange(int line, float left, float right)
		{
			this.Left[line] = left;
			this.Right[line] = right;
		}

		public void CopyTo(Space dest, int start, int length)
		{
			Array.Copy(this.Left, start, dest.Left, start, length);
			Array.Copy(this.Right, start, dest.Right, start, length);
		}

		internal void LeftSmooth(float left, int line)
		{
			float l = left;
			for (int i = line - 1; i >= 0; i--)
			{
				l -= smoothStep;
				if (this.Left[i] >= l)
					return;
				this.Left[i] = l;
			}
		}

		internal void RightSmooth(float right, int line)
		{
			float r = right;
			for (int i = line - 1; i >= 0; i--)
			{
				r += smoothStep;
				if (this.Right[i] <= r)
					return;
				this.Right[i] = r;
			}
		}
	}
}
