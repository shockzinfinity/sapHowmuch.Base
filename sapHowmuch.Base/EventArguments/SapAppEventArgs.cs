using SAPbouiCOM;
using System;
using System.Text;

namespace sapHowmuch.Base.EventArguments
{
	public class SapAppEventArgs : EventArgs, ISapEventArgs<BoAppEventTypes>
	{
		public DateTime EventFiredTime { get; private set; }
		public BoAppEventTypes DetailArg { get; private set; }

		public SapAppEventArgs(DateTime firedTime, BoAppEventTypes pVal)
		{
			this.EventFiredTime = firedTime;
			this.DetailArg = pVal;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append(" | ")
				.Append(DetailArg);

			return sb.ToString();
		}
	}
}