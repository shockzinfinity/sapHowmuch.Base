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
			sb.Append(EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
			sb.Append(" | ");
			sb.Append(BubbleEvent);
			sb.Append(" | ");
			sb.Append(DetailArg.BeforeAction);
			sb.Append(" | ");
			sb.Append(DetailArg.ActionSuccess);
			sb.Append(" | ");
			sb.Append(DetailArg.EventType);
			sb.Append(" | ");
			sb.Append(DetailArg.FormMode);
			sb.Append(" | ");
			sb.Append(DetailArg.FormUID);
			sb.Append(" | ");
			sb.Append(DetailArg.FormTypeEx);
			sb.Append(" | ");
			sb.Append(DetailArg.FormTypeCount);
			sb.Append(" | ");
			sb.Append(DetailArg.FormUID);
			sb.Append(" | ");
			sb.Append(DetailArg.ItemUID);
			sb.Append(" | ");
			sb.Append(DetailArg.ColUID);
			sb.Append(" | ");
			sb.Append(DetailArg.Row);
			sb.Append(" | ");
			sb.Append(DetailArg.CharPressed);
			sb.Append(" | ");
			sb.Append(DetailArg.Modifiers);
			sb.Append(" | ");
			sb.Append(DetailArg.ItemChanged);
			sb.Append(" | ");
			sb.Append(DetailArg.InnerEvent);
			sb.Append(" | ");
			sb.Append(DetailArg.PopUpIndicator);

			return sb.ToString();
		}
	}
}