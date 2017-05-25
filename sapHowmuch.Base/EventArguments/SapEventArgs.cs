using System;

namespace sapHowmuch.Base.EventArguments
{
	public class SapEventArgs<T> : EventArgs
	{
		public DateTime EventFiredTime { get; private set; }
		public string FormUid { get; private set; }
		public T DetailArg { get; private set; }
		public bool BubbleEvent { get; private set; }

		public SapEventArgs(DateTime firedTime, string formUid, T pval, bool bubble)
		{
			this.EventFiredTime = firedTime;
			this.FormUid = formUid;
			this.DetailArg = pval;
			this.BubbleEvent = bubble;
		}
	}
}