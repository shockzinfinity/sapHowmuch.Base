using System;
using System.Text;

namespace sapHowmuch.Base.EventArguments
{
	public class SapEventArgs<T> : EventArgs, ISapEventArgs<T>
	{
		public DateTime EventFiredTime { get; private set; }
		public T DetailArg { get; private set; }
		public bool BubbleEvent { get; private set; }

		public SapEventArgs(DateTime firedTime, T pval, bool bubble)
		{
			this.EventFiredTime = firedTime;
			this.DetailArg = pval;
			this.BubbleEvent = bubble;
		}
	}
}