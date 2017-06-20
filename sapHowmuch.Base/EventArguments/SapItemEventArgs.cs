using SAPbouiCOM;
using System;
using System.Text;

namespace sapHowmuch.Base.EventArguments
{
	public class SapItemEventArgs : SapEventArgs<ItemEvent>
	{
		public SapItemEventArgs(DateTime firedTime, ItemEvent pval, bool bubble)
			: base(firedTime, pval, bubble)
		{
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append(" | ")
			.Append(BubbleEvent).Append(" | ")
			.Append(DetailArg.BeforeAction).Append(" | ")
			.Append(DetailArg.ActionSuccess).Append(" | ")
			.Append(DetailArg.EventType).Append(" | ")
			.Append(DetailArg.FormMode).Append(" | ")
			.Append(DetailArg.FormUID).Append(" | ")
			.Append(DetailArg.FormTypeEx).Append(" | ")
			.Append(DetailArg.FormTypeCount).Append(" | ")
			.Append(DetailArg.FormUID).Append(" | ")
			.Append(DetailArg.ItemUID).Append(" | ")
			.Append(DetailArg.ColUID).Append(" | ")
			.Append(DetailArg.Row).Append(" | ")
			.Append(DetailArg.CharPressed).Append(" | ")
			.Append(DetailArg.Modifiers).Append(" | ")
			.Append(DetailArg.ItemChanged).Append(" | ")
			.Append(DetailArg.InnerEvent).Append(" | ")
			.Append(DetailArg.PopUpIndicator);

			return sb.ToString();
		}
	}
}