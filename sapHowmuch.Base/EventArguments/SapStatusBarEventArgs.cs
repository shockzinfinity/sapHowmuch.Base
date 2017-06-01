using SAPbouiCOM;
using System;

namespace sapHowmuch.Base.EventArguments
{
	public class SapStatusBarEventArgs : EventArgs, ISapEventArgs<BoStatusBarMessageType>
	{
		public DateTime EventFiredTime { get; private set; }
		public BoStatusBarMessageType DetailArg { get; private set; }
		public string Message { get; private set; }

		public SapStatusBarEventArgs(DateTime firedTime, string message, BoStatusBarMessageType pval)
		{
			this.EventFiredTime = firedTime;
			this.DetailArg = pval;
			this.Message = message;
		}
	}
}