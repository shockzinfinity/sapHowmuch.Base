using SAPbouiCOM;
using System;

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
	}
}