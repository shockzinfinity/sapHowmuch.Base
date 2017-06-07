using sapHowmuch.Base.EventArguments;
using System;
using System.Reactive.Linq;

namespace sapHowmuch.Base
{
	/// <summary>
	/// This is a partial <c>SapStream</c> class.
	/// This provides Observable stream from SAP Business One UI API events.
	/// </summary>
	partial class SapStream
	{
		#region SAP Business One event stream

		public static IObservable<SapAppEventArgs> AppEventStream { get; private set; }
		public static IObservable<SapStatusBarEventArgs> StatusBarEventStream { get; private set; }
		public static IObservable<SapItemEventArgs> ItemEventStream { get; private set; }
		public static IObservable<SapEventArgs<SAPbouiCOM.MenuEvent>> MenuEventStream { get; private set; }
		public static IObservable<SapEventArgs<SAPbouiCOM.BusinessObjectInfo>> FormDataEventStream { get; private set; }
		public static IObservable<SapEventArgs<SAPbouiCOM.LayoutKeyInfo>> LayoutKeyInfoStream { get; private set; }
		public static IObservable<SapEventArgs<SAPbouiCOM.PrintEventInfo>> PrintEventStream { get; private set; }
		public static IObservable<SapEventArgs<SAPbouiCOM.ReportDataInfo>> ReportDataInfoEventStream { get; private set; }
		public static IObservable<SapEventArgs<SAPbouiCOM.ProgressBarEvent>> ProgressBarEventStream { get; private set; }
		public static IObservable<SapEventArgs<SAPbouiCOM.ContextMenuInfo>> RightClickEventStream { get; private set; }
		public static IObservable<SapEventArgs<SAPbouiCOM.B1iEvent>> ServerInvokeCompletedEventStream { get; private set; }
		public static IObservable<SapEventArgs<SAPbouiCOM.UDOEvent>> UDOEventStream { get; private set; }
		public static IObservable<SapEventArgs<SAPbouiCOM.WidgetData>> WidgetEventStream { get; private set; }

		#endregion SAP Business One event stream

		#region initialize methods

		private static bool InitializeSboApplication()
		{
			try
			{
				if (_application != null)
				{
					// app event 등록

					AppEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_AppEventEventHandler, SapAppEventArgs>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_AppEventEventHandler iHandler = (SAPbouiCOM.BoAppEventTypes eventTypes) =>
							{
								SapAppEventArgs appEventArgs = new SapAppEventArgs(DateTime.Now, eventTypes);
								handler.Invoke(appEventArgs);
							};

							return iHandler;
						},
						handler => _application.AppEvent += handler,
						handler => _application.AppEvent -= handler);

					ItemEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_ItemEventEventHandler, SapItemEventArgs>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_ItemEventEventHandler iHandler = (string formUid, ref SAPbouiCOM.ItemEvent pval, out bool bubble) =>
							{
								bubble = true;
								SapItemEventArgs itemArgs = new SapItemEventArgs(DateTime.Now, formUid, pval, bubble);
								handler.Invoke(itemArgs);
								bubble = itemArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.ItemEvent += handler,
						handler => _application.ItemEvent -= handler);

					StatusBarEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_StatusBarEventEventHandler, SapStatusBarEventArgs>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_StatusBarEventEventHandler iHandler = (string text, SAPbouiCOM.BoStatusBarMessageType messageType) =>
							{
								SapStatusBarEventArgs statusBarEventArgs = new SapStatusBarEventArgs(DateTime.Now, text, messageType);
								handler.Invoke(statusBarEventArgs);
							};

							return iHandler;
						},
						handler => _application.StatusBarEvent += handler,
						handler => _application.StatusBarEvent -= handler);

					FormDataEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler, SapEventArgs<SAPbouiCOM.BusinessObjectInfo>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler iHandler = (ref SAPbouiCOM.BusinessObjectInfo pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.BusinessObjectInfo> formDataArgs = new SapEventArgs<SAPbouiCOM.BusinessObjectInfo>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(formDataArgs);
								BubbleEvent = formDataArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.FormDataEvent += handler,
						handler => _application.FormDataEvent -= handler);

					LayoutKeyInfoStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_LayoutKeyEventEventHandler, SapEventArgs<SAPbouiCOM.LayoutKeyInfo>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_LayoutKeyEventEventHandler iHandler = (ref SAPbouiCOM.LayoutKeyInfo pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.LayoutKeyInfo> layoutArgs = new SapEventArgs<SAPbouiCOM.LayoutKeyInfo>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(layoutArgs);
								BubbleEvent = layoutArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.LayoutKeyEvent += handler,
						handler => _application.LayoutKeyEvent -= handler);

					MenuEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_MenuEventEventHandler, SapEventArgs<SAPbouiCOM.MenuEvent>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_MenuEventEventHandler iHandler = (ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent) =>
							{
								// NOTE: Note that every MenuEvent event is sent twice: first, before the SAP Business One application handles it
								// and second after the SAP Business One application handled it.
								// To prevent double handling of the event, use one of the following two options:
								// ItemEvent.BeforeAction (place your action in an If clause.
								// Set BubbleEvent to False (the event will be sent only once.
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.MenuEvent> menuArgs = new SapEventArgs<SAPbouiCOM.MenuEvent>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(menuArgs);
								BubbleEvent = menuArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.MenuEvent += handler,
						handler => _application.MenuEvent -= handler);

					PrintEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_PrintEventEventHandler, SapEventArgs<SAPbouiCOM.PrintEventInfo>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_PrintEventEventHandler iHandler = (ref SAPbouiCOM.PrintEventInfo pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.PrintEventInfo> printArgs = new SapEventArgs<SAPbouiCOM.PrintEventInfo>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(printArgs);
								BubbleEvent = printArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.PrintEvent += handler,
						handler => _application.PrintEvent -= handler);

					ProgressBarEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_ProgressBarEventEventHandler, SapEventArgs<SAPbouiCOM.ProgressBarEvent>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_ProgressBarEventEventHandler iHandler = (ref SAPbouiCOM.ProgressBarEvent pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.ProgressBarEvent> progressArgs = new SapEventArgs<SAPbouiCOM.ProgressBarEvent>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(progressArgs);
								BubbleEvent = progressArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.ProgressBarEvent += handler,
						handler => _application.ProgressBarEvent -= handler);

					ReportDataInfoEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_ReportDataEventEventHandler, SapEventArgs<SAPbouiCOM.ReportDataInfo>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_ReportDataEventEventHandler iHandler = (ref SAPbouiCOM.ReportDataInfo pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.ReportDataInfo> reportArgs = new SapEventArgs<SAPbouiCOM.ReportDataInfo>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(reportArgs);
								BubbleEvent = reportArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.ReportDataEvent += handler,
						handler => _application.ReportDataEvent -= handler);

					RightClickEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler, SapEventArgs<SAPbouiCOM.ContextMenuInfo>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler iHandler = (ref SAPbouiCOM.ContextMenuInfo pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.ContextMenuInfo> rightArgs = new SapEventArgs<SAPbouiCOM.ContextMenuInfo>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(rightArgs);
								BubbleEvent = rightArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.RightClickEvent += handler,
						handler => _application.RightClickEvent -= handler);

					ServerInvokeCompletedEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_ServerInvokeCompletedEventEventHandler, SapEventArgs<SAPbouiCOM.B1iEvent>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_ServerInvokeCompletedEventEventHandler iHandler = (ref SAPbouiCOM.B1iEvent pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.B1iEvent> b1iArgs = new SapEventArgs<SAPbouiCOM.B1iEvent>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(b1iArgs);
								BubbleEvent = b1iArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.ServerInvokeCompletedEvent += handler,
						handler => _application.ServerInvokeCompletedEvent -= handler);

					UDOEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_UDOEventEventHandler, SapEventArgs<SAPbouiCOM.UDOEvent>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_UDOEventEventHandler iHandler = (ref SAPbouiCOM.UDOEvent pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.UDOEvent> udoArgs = new SapEventArgs<SAPbouiCOM.UDOEvent>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(udoArgs);
								BubbleEvent = udoArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.UDOEvent += handler,
						handler => _application.UDOEvent -= handler);

					WidgetEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_WidgetEventEventHandler, SapEventArgs<SAPbouiCOM.WidgetData>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_WidgetEventEventHandler iHandler = (ref SAPbouiCOM.WidgetData pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.WidgetData> widgetArgs = new SapEventArgs<SAPbouiCOM.WidgetData>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(widgetArgs);
								BubbleEvent = widgetArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.WidgetEvent += handler,
						handler => _application.WidgetEvent -= handler);
				}
				else
				{
					AppEventStream = Observable.Empty<SapAppEventArgs>();
					StatusBarEventStream = Observable.Empty<SapStatusBarEventArgs>();
					ItemEventStream = Observable.Empty<SapItemEventArgs>();
					MenuEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.MenuEvent>>();
					FormDataEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.BusinessObjectInfo>>();
					LayoutKeyInfoStream = Observable.Empty<SapEventArgs<SAPbouiCOM.LayoutKeyInfo>>();
					PrintEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.PrintEventInfo>>();
					ReportDataInfoEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.ReportDataInfo>>();
					ProgressBarEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.ProgressBarEvent>>();
					RightClickEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.ContextMenuInfo>>();
					ServerInvokeCompletedEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.B1iEvent>>();
					UDOEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.UDOEvent>>();
					WidgetEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.WidgetData>>();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return true;
		}

		#endregion initialize methods
	}
}