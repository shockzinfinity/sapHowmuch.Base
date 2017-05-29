using sapHowmuch.Base.EventArguments;
using System;

namespace sapHowmuch.Base
{
	public interface ISapStream
	{
		bool IsConnectToUI { get; }
		SAPbouiCOM.Application UiApp { get; }
		SAPbobsCOM.Company DICompany { get; }

		IObservable<SapAppEventArgs> AppEventStream { get; }
		IObservable<SapStatusBarEventArgs> StatusBarEventStream { get; }
		IObservable<SapItemEventArgs> ItemEventStream { get; }
		IObservable<SapEventArgs<SAPbouiCOM.MenuEvent>> MenuEventStream { get; }
		IObservable<SapEventArgs<SAPbouiCOM.BusinessObjectInfo>> FormDataEventStream { get; }
		IObservable<SapEventArgs<SAPbouiCOM.LayoutKeyInfo>> LayoutKeyInfoStream { get; }
		IObservable<SapEventArgs<SAPbouiCOM.PrintEventInfo>> PrintEventStream { get; }
		IObservable<SapEventArgs<SAPbouiCOM.ReportDataInfo>> ReportDataInfoEventStream { get; }
		IObservable<SapEventArgs<SAPbouiCOM.ProgressBarEvent>> ProgressBarEventStream { get; }
		IObservable<SapEventArgs<SAPbouiCOM.ContextMenuInfo>> RightClickEventStream { get; }
		IObservable<SapEventArgs<SAPbouiCOM.B1iEvent>> ServerInvokeCompletedEventStream { get; }
		IObservable<SapEventArgs<SAPbouiCOM.UDOEvent>> UDOEventStream { get; }
		IObservable<SapEventArgs<SAPbouiCOM.WidgetData>> WidgetEventStream { get; }
	}
}