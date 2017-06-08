using SAPbouiCOM;
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
		public static IObservable<SapEventArgs<MenuEvent>> MenuEventStream { get; private set; }
		public static IObservable<SapEventArgs<BusinessObjectInfo>> FormDataEventStream { get; private set; }
		public static IObservable<SapEventArgs<LayoutKeyInfo>> LayoutKeyInfoStream { get; private set; }
		public static IObservable<SapEventArgs<PrintEventInfo>> PrintEventStream { get; private set; }
		public static IObservable<SapEventArgs<ReportDataInfo>> ReportDataInfoEventStream { get; private set; }
		public static IObservable<SapEventArgs<ProgressBarEvent>> ProgressBarEventStream { get; private set; }
		public static IObservable<SapEventArgs<ContextMenuInfo>> RightClickEventStream { get; private set; }
		public static IObservable<SapEventArgs<B1iEvent>> ServerInvokeCompletedEventStream { get; private set; }
		public static IObservable<SapEventArgs<UDOEvent>> UDOEventStream { get; private set; }
		public static IObservable<SapEventArgs<WidgetData>> WidgetEventStream { get; private set; }

		#endregion SAP Business One event stream

		#region initialize methods

		private static bool InitializeSboApplication()
		{
			try
			{
				if (_application != null)
				{
					// app event 등록

					AppEventStream = Observable.FromEvent<_IApplicationEvents_AppEventEventHandler, SapAppEventArgs>(
						handler =>
						{
							_IApplicationEvents_AppEventEventHandler iHandler = (BoAppEventTypes eventTypes) =>
							{
								SapAppEventArgs appEventArgs = new SapAppEventArgs(DateTime.Now, eventTypes);
								handler.Invoke(appEventArgs);
							};

							return iHandler;
						},
						handler => _application.AppEvent += handler,
						handler => _application.AppEvent -= handler);

					ItemEventStream = Observable.FromEvent<_IApplicationEvents_ItemEventEventHandler, SapItemEventArgs>(
						handler =>
						{
							_IApplicationEvents_ItemEventEventHandler iHandler = (string formUid, ref ItemEvent pval, out bool bubble) =>
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

					StatusBarEventStream = Observable.FromEvent<_IApplicationEvents_StatusBarEventEventHandler, SapStatusBarEventArgs>(
						handler =>
						{
							_IApplicationEvents_StatusBarEventEventHandler iHandler = (string text, BoStatusBarMessageType messageType) =>
							{
								SapStatusBarEventArgs statusBarEventArgs = new SapStatusBarEventArgs(DateTime.Now, text, messageType);
								handler.Invoke(statusBarEventArgs);
							};

							return iHandler;
						},
						handler => _application.StatusBarEvent += handler,
						handler => _application.StatusBarEvent -= handler);

					FormDataEventStream = Observable.FromEvent<_IApplicationEvents_FormDataEventEventHandler, SapEventArgs<BusinessObjectInfo>>(
						handler =>
						{
							_IApplicationEvents_FormDataEventEventHandler iHandler = (ref BusinessObjectInfo pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<BusinessObjectInfo> formDataArgs = new SapEventArgs<BusinessObjectInfo>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(formDataArgs);
								BubbleEvent = formDataArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.FormDataEvent += handler,
						handler => _application.FormDataEvent -= handler);

					LayoutKeyInfoStream = Observable.FromEvent<_IApplicationEvents_LayoutKeyEventEventHandler, SapEventArgs<LayoutKeyInfo>>(
						handler =>
						{
							_IApplicationEvents_LayoutKeyEventEventHandler iHandler = (ref LayoutKeyInfo pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<LayoutKeyInfo> layoutArgs = new SapEventArgs<LayoutKeyInfo>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(layoutArgs);
								BubbleEvent = layoutArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.LayoutKeyEvent += handler,
						handler => _application.LayoutKeyEvent -= handler);

					MenuEventStream = Observable.FromEvent<_IApplicationEvents_MenuEventEventHandler, SapEventArgs<MenuEvent>>(
						handler =>
						{
							_IApplicationEvents_MenuEventEventHandler iHandler = (ref MenuEvent pVal, out bool BubbleEvent) =>
							{
								// NOTE: Note that every MenuEvent event is sent twice: first, before the SAP Business One application handles it
								// and second after the SAP Business One application handled it.
								// To prevent double handling of the event, use one of the following two options:
								// ItemEvent.BeforeAction (place your action in an If clause.
								// Set BubbleEvent to False (the event will be sent only once.
								BubbleEvent = true;
								SapEventArgs<MenuEvent> menuArgs = new SapEventArgs<MenuEvent>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(menuArgs);
								BubbleEvent = menuArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.MenuEvent += handler,
						handler => _application.MenuEvent -= handler);

					PrintEventStream = Observable.FromEvent<_IApplicationEvents_PrintEventEventHandler, SapEventArgs<PrintEventInfo>>(
						handler =>
						{
							_IApplicationEvents_PrintEventEventHandler iHandler = (ref PrintEventInfo pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<PrintEventInfo> printArgs = new SapEventArgs<PrintEventInfo>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(printArgs);
								BubbleEvent = printArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.PrintEvent += handler,
						handler => _application.PrintEvent -= handler);

					ProgressBarEventStream = Observable.FromEvent<_IApplicationEvents_ProgressBarEventEventHandler, SapEventArgs<ProgressBarEvent>>(
						handler =>
						{
							_IApplicationEvents_ProgressBarEventEventHandler iHandler = (ref ProgressBarEvent pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<ProgressBarEvent> progressArgs = new SapEventArgs<ProgressBarEvent>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(progressArgs);
								BubbleEvent = progressArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.ProgressBarEvent += handler,
						handler => _application.ProgressBarEvent -= handler);

					ReportDataInfoEventStream = Observable.FromEvent<_IApplicationEvents_ReportDataEventEventHandler, SapEventArgs<ReportDataInfo>>(
						handler =>
						{
							_IApplicationEvents_ReportDataEventEventHandler iHandler = (ref ReportDataInfo pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<ReportDataInfo> reportArgs = new SapEventArgs<ReportDataInfo>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(reportArgs);
								BubbleEvent = reportArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.ReportDataEvent += handler,
						handler => _application.ReportDataEvent -= handler);

					RightClickEventStream = Observable.FromEvent<_IApplicationEvents_RightClickEventEventHandler, SapEventArgs<ContextMenuInfo>>(
						handler =>
						{
							_IApplicationEvents_RightClickEventEventHandler iHandler = (ref ContextMenuInfo pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<ContextMenuInfo> rightArgs = new SapEventArgs<ContextMenuInfo>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(rightArgs);
								BubbleEvent = rightArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.RightClickEvent += handler,
						handler => _application.RightClickEvent -= handler);

					ServerInvokeCompletedEventStream = Observable.FromEvent<_IApplicationEvents_ServerInvokeCompletedEventEventHandler, SapEventArgs<B1iEvent>>(
						handler =>
						{
							_IApplicationEvents_ServerInvokeCompletedEventEventHandler iHandler = (ref B1iEvent pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<B1iEvent> b1iArgs = new SapEventArgs<B1iEvent>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(b1iArgs);
								BubbleEvent = b1iArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.ServerInvokeCompletedEvent += handler,
						handler => _application.ServerInvokeCompletedEvent -= handler);

					UDOEventStream = Observable.FromEvent<_IApplicationEvents_UDOEventEventHandler, SapEventArgs<UDOEvent>>(
						handler =>
						{
							_IApplicationEvents_UDOEventEventHandler iHandler = (ref UDOEvent pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<UDOEvent> udoArgs = new SapEventArgs<UDOEvent>(DateTime.Now, pVal, BubbleEvent);
								handler.Invoke(udoArgs);
								BubbleEvent = udoArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.UDOEvent += handler,
						handler => _application.UDOEvent -= handler);

					WidgetEventStream = Observable.FromEvent<_IApplicationEvents_WidgetEventEventHandler, SapEventArgs<WidgetData>>(
						handler =>
						{
							_IApplicationEvents_WidgetEventEventHandler iHandler = (ref WidgetData pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<WidgetData> widgetArgs = new SapEventArgs<WidgetData>(DateTime.Now, pVal, BubbleEvent);
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
					MenuEventStream = Observable.Empty<SapEventArgs<MenuEvent>>();
					FormDataEventStream = Observable.Empty<SapEventArgs<BusinessObjectInfo>>();
					LayoutKeyInfoStream = Observable.Empty<SapEventArgs<LayoutKeyInfo>>();
					PrintEventStream = Observable.Empty<SapEventArgs<PrintEventInfo>>();
					ReportDataInfoEventStream = Observable.Empty<SapEventArgs<ReportDataInfo>>();
					ProgressBarEventStream = Observable.Empty<SapEventArgs<ProgressBarEvent>>();
					RightClickEventStream = Observable.Empty<SapEventArgs<ContextMenuInfo>>();
					ServerInvokeCompletedEventStream = Observable.Empty<SapEventArgs<B1iEvent>>();
					UDOEventStream = Observable.Empty<SapEventArgs<UDOEvent>>();
					WidgetEventStream = Observable.Empty<SapEventArgs<WidgetData>>();
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