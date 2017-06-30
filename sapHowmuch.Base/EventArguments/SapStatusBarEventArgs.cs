using SAPbouiCOM;
using System;
using System.Text;

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

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append(" | ")
				.Append(DetailArg.ToString()).Append(" | ")
				.Append(Message);

			return sb.ToString();
		}
	}
}