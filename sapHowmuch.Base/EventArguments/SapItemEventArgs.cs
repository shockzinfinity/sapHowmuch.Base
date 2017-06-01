using SAPbouiCOM;
using System;

namespace sapHowmuch.Base.EventArguments
{
	public class SapItemEventArgs : SapEventArgs<ItemEvent>
	{
		public string FormUid { get; private set; }

		public SapItemEventArgs(DateTime firedTime, string formUid, ItemEvent pval, bool bubble)
			: base(firedTime, pval, bubble)
		{
			this.FormUid = formUid;
		}
	}
}