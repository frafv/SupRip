using System;
namespace SupRip
{
	internal class SRTInfo
	{
		public int unscanned;
		public int finished;
		public int containingErrors;
		public SRTInfo()
		{
			this.unscanned = 0;
			this.finished = 0;
			this.containingErrors = 0;
		}
	}
}
