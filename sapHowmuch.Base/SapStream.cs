using sapHowmuch.Base.EventArguments;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;

namespace sapHowmuch.Base
{
	public sealed class SapStream : ISapStream, IDisposable
	{
		private SAPbobsCOM.Company _company; // di company
		private SAPbouiCOM.Application _application; // ui application

		/// <summary>
		/// SAP DI Company instance
		/// </summary>
		public SAPbobsCOM.Company DICompany
		{
			get
			{
				if (_company == null || !_company.Connected)
				{
					ConnectDI();
				}

				return _company;
			}
		}

		/// <summary>
		/// is connected ui app
		/// </summary>
		public bool IsConnectToUI { get { return _application != null; } }

		/// <summary>
		/// SAP Business One UI API
		/// </summary>
		public SAPbouiCOM.Application UiApp { get { return _application; } }

		#region SAP Business One event stream

		public IObservable<SapAppEventArgs> AppEventStream { get; private set; }
		public IObservable<SapStatusBarEventArgs> StatusBarEventStream { get; private set; }
		public IObservable<SapItemEventArgs> ItemEventStream { get; private set; }
		public IObservable<SapEventArgs<SAPbouiCOM.MenuEvent>> MenuEventStream { get; private set; }
		public IObservable<SapEventArgs<SAPbouiCOM.BusinessObjectInfo>> FormDataEventStream { get; private set; }
		public IObservable<SapEventArgs<SAPbouiCOM.LayoutKeyInfo>> LayoutKeyInfoStream { get; private set; }
		public IObservable<SapEventArgs<SAPbouiCOM.PrintEventInfo>> PrintEventStream { get; private set; }
		public IObservable<SapEventArgs<SAPbouiCOM.ReportDataInfo>> ReportDataInfoEventStream { get; private set; }
		public IObservable<SapEventArgs<SAPbouiCOM.ProgressBarEvent>> ProgressBarEventStream { get; private set; }
		public IObservable<SapEventArgs<SAPbouiCOM.ContextMenuInfo>> RightClickEventStream { get; private set; }
		public IObservable<SapEventArgs<SAPbouiCOM.B1iEvent>> ServerInvokeCompletedEventStream { get; private set; }
		public IObservable<SapEventArgs<SAPbouiCOM.UDOEvent>> UDOEventStream { get; private set; }
		public IObservable<SapEventArgs<SAPbouiCOM.WidgetData>> WidgetEventStream { get; private set; }

		#endregion SAP Business One event stream

		#region various SAP connection methods

		// DI 를 통한 직접 연결
		private int ConnectDI()
		{
			int retVal = -1;
			_company = new SAPbobsCOM.Company();
			_company.Server = ConfigurationManager.AppSettings["sapServer"];
			_company.DbServerType = (SAPbobsCOM.BoDataServerTypes)Enum.Parse(typeof(SAPbobsCOM.BoDataServerTypes), ConfigurationManager.AppSettings["sapDBType"]);
			_company.CompanyDB = ConfigurationManager.AppSettings["sapCompanyDBName"];
			_company.LicenseServer = ConfigurationManager.AppSettings["sapLicenseServer"];
			_company.UserName = ConfigurationManager.AppSettings["sapUserName"];
			_company.Password = ConfigurationManager.AppSettings["sapPassword"];

			try
			{
				retVal = _company.Connect();

				if (retVal == 0)
				{
					if (ProcessHelper.ByName(Constants.SapUiAppName).Count() > 0)
					{
						// if ui api could not find connection string, raise exception
						// SAP 로그인 화면만 떠도, 연결됨. (cache 때문인가?)
						var uiApp = new SAPbouiCOM.Framework.Application();
						_application = SAPbouiCOM.Framework.Application.SBO_Application;

						if (_application == null)
						{
							throw new ArgumentNullException(nameof(_application));
						}

						InitializeSboApplication();
					}
					else
					{
						Debug.WriteLine("SAP Business One UI is not found.");
					}
				}
				else
				{
					throw new InvalidOperationException($"ErrorCode: {_company.GetLastErrorCode()}, ErrorDescription: {_company.GetLastErrorDescription()}");
				}
			}
			catch
			{
				throw;
			}

			return retVal;
		}

		private bool InitializeSboApplication()
		{
			try
			{
				if (_application != null)
				{
					// app event 등록

					this.AppEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_AppEventEventHandler, SapAppEventArgs>(
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

					this.ItemEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_ItemEventEventHandler, SapItemEventArgs>(
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

					this.StatusBarEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_StatusBarEventEventHandler, SapStatusBarEventArgs>(
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

					this.FormDataEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler, SapEventArgs<SAPbouiCOM.BusinessObjectInfo>>(
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

					this.LayoutKeyInfoStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_LayoutKeyEventEventHandler, SapEventArgs<SAPbouiCOM.LayoutKeyInfo>>(
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

					this.MenuEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_MenuEventEventHandler, SapEventArgs<SAPbouiCOM.MenuEvent>>(
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

					this.PrintEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_PrintEventEventHandler, SapEventArgs<SAPbouiCOM.PrintEventInfo>>(
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

					this.ProgressBarEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_ProgressBarEventEventHandler, SapEventArgs<SAPbouiCOM.ProgressBarEvent>>(
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

					this.ReportDataInfoEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_ReportDataEventEventHandler, SapEventArgs<SAPbouiCOM.ReportDataInfo>>(
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

					this.RightClickEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler, SapEventArgs<SAPbouiCOM.ContextMenuInfo>>(
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

					this.ServerInvokeCompletedEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_ServerInvokeCompletedEventEventHandler, SapEventArgs<SAPbouiCOM.B1iEvent>>(
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

					this.UDOEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_UDOEventEventHandler, SapEventArgs<SAPbouiCOM.UDOEvent>>(
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

					this.WidgetEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_WidgetEventEventHandler, SapEventArgs<SAPbouiCOM.WidgetData>>(
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
					this.AppEventStream = Observable.Empty<SapAppEventArgs>();
					this.StatusBarEventStream = Observable.Empty<SapStatusBarEventArgs>();
					this.ItemEventStream = Observable.Empty<SapItemEventArgs>();
					this.MenuEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.MenuEvent>>();
					this.FormDataEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.BusinessObjectInfo>>();
					this.LayoutKeyInfoStream = Observable.Empty<SapEventArgs<SAPbouiCOM.LayoutKeyInfo>>();
					this.PrintEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.PrintEventInfo>>();
					this.ReportDataInfoEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.ReportDataInfo>>();
					this.ProgressBarEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.ProgressBarEvent>>();
					this.RightClickEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.ContextMenuInfo>>();
					this.ServerInvokeCompletedEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.B1iEvent>>();
					this.UDOEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.UDOEvent>>();
					this.WidgetEventStream = Observable.Empty<SapEventArgs<SAPbouiCOM.WidgetData>>();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return true;
		}

		// UI 를 통한 연결
		private int ConnectUI(string connectionString = "")
		{
			int retVal = 0;

			if (string.IsNullOrWhiteSpace(connectionString))
			{
				// if DEBUG mode
				connectionString = Constants.SapUiDebugConnectionString;
			}

			var sbGuiApi = new SAPbouiCOM.SboGuiApi();
			sbGuiApi.Connect(connectionString);

			try
			{
				_application = sbGuiApi.GetApplication();
			}
			catch (Exception ex)
			{
				var message = string.Format(CultureInfo.InvariantCulture, "{0} Initialization - Error accessing SBO: {1}", "DB_TestConnection", ex.Message);
				Console.WriteLine(message);
				retVal = -1;
			}
			finally
			{
				Marshal.FinalReleaseComObject(sbGuiApi);
			}

			return retVal;
		}

		private int ConnectUIDI(string connectionString = "")
		{
			int retVal = ConnectUI(connectionString);
			if (retVal == 0)
			{
				try
				{
					_company = _application.Company.GetDICompany() as SAPbobsCOM.Company;
				}
				catch (Exception ex)
				{
					var message = string.Format(CultureInfo.InvariantCulture, "{0} Initialization - Error accessing SBO: {1}", "db_TestConnection", ex.Message);
					Console.WriteLine(message);
					retVal = -1;
				}
			}

			return retVal;
		}

		// deriving a connection
		private int DerivingConnect()
		{
			int uidiConnect = ConnectUIDI();
			int retVal = 0;

			if (uidiConnect == 0)
			{
				SAPbobsCOM.Company company2 = new SAPbobsCOM.Company();

				try
				{
					var cookie = company2.GetContextCookie();
					var context = _application.Company.GetConnectionContext(cookie);
					company2.SetSboLoginContext(context);
					company2.CompanyDB = "DB_TestConnection2";
					retVal = company2.Connect();

					if (retVal == 0)
					{
						Console.WriteLine("Gui connected to {0}", _company.CompanyName);
						Console.WriteLine("New connection to {0}", company2.CompanyName);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				finally
				{
					Marshal.FinalReleaseComObject(company2);
				}
			}

			return retVal;
		}

		#region Connect to DI-Server

		//private static int LastErrorCode;
		//private static string LastErrorDescription;
		//private static readonly Node DisObject = new Node();
		//private static string ConectDIS()
		//{
		//	string returnValue = string.Empty;
		//	var enveloppeLogon = "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><env:Body><dis:Login xmlns:dis=\"http://www.sap.com/SBO/DIS\"><DatabaseServer>{0}</DatabaseServer><DatabaseName>{1}</DatabaseName><DatabaseType>{2}</DatabaseType><CompanyUsername&gt:{3}</CompanyUsername><CompanyPassword>{4}</CompanyPassword><Language>{5}</Language><LicenseServer>{6}</LicenseServer></dis:Login></env:Body></env:Envelope>";
		//	var xmlString = string.Format(CultureInfo.InvariantCulture,
		//		enveloppeLogon,
		//		"192.168.90.118",
		//		"DB_TestConnection",
		//		"dst_MSSQL2008",
		//		"manager",
		//		"password",
		//		"ln_English",
		//		"192.168.90.118:30000");
		//	var sb = new StringBuilder();
		//	sb.Append("<?xml version=\"1.0\" ?>").Append(xmlString);
		//	var soapMessage = sb.ToString();
		//	returnValue = DisObject.Interact(soapMessage);
		//	if (0 == LastErrorCode)
		//	{
		//		var xmlDocument = new XmlDocument();
		//		xmlDocument.LoadXml(returnValue);
		//		var sessionNode = xmlDocument.SelectSingleNode("//*[local-name()='SessionID']");
		//		if (null != sessionNode)
		//		{
		//			returnValue = sessionNode.InnerText;
		//			Console.WriteLine("Connected to DIS. SessionId: {0}", returnValue);
		//		}
		//	}
		//	return returnValue;
		//}

		#endregion Connect to DI-Server

		#region Connect to DIS thru B1WS

		//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:log="LoginService">
		//	<soapenv:Header/>
		//	<soapenv:Body>
		//		<log:Login>
		//			<log:DatabaseServer>192.168.90.118</log:DatabaseServer>
		//			<log:DatabaseName>DB_TestConnection</log:DatabaseName>
		//			<log:DatabaseType>dst_MSSQL2008</log:DatabaseType>
		//			<log:CompanyUsername>manager</log:CompanyUsername>
		//			<log:CompanyPassword>Kuldip</log:CompanyPassword>
		//			<log:Language>ln_English</log:Language>
		//			<log:LicenseServer>192.168.90.118:30000</log:LicenseServer>
		//		</log:Login>
		//	</soapenv:Body>
		//</soapenv:Envelope>

		#endregion Connect to DIS thru B1WS

		#endregion various SAP connection methods

		#region IDisposable implementation

		private bool _disposed;

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// exlicit dispose
		/// </summary>
		/// <remarks>
		/// sealed 클래스가 아닐 경우,
		/// protected virtual 로...
		/// </remarks>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			if (this._disposed)
			{
				return;
			}

			try
			{
				if (disposing)
				{
					// 관리자원은 여기서 해제
					// IDisposable 인터페이스를 구현하는 멤버들을 여기서 정리합니다.
					//IDisposable[] targetList = new IDisposable[this.items.Count];
					//this.items.CopyTo(targetList);
					//foreach (IDisposable eachItem in targetList)
					//{
					//	try { eachItem.Dispose(); }
					//	catch (Exception ex) { /* 예외 처리를 수행합니다. */ }
					//	finally { /* 정리 작업을 수행합니다. */ }
					//}
					//this.items.Clear();
				}

				// 비관리자원은 여기서 해제
				//try { /* .NET Framework에 의하여 관리되지 않는 외부 리소스들을 여기서 정리합니다. */ }
				//catch { /* 예외 처리를 수행합니다. */ }
				//finally
				//{
				//	/* 정리 작업을 수행합니다.  */
				//this._disposed = true;
				//}

				if (_company.Connected)
				{
					_company.Disconnect();
				}

				Marshal.FinalReleaseComObject(_company);
				Marshal.FinalReleaseComObject(_application);

				this._disposed = true;
			}
			finally
			{
				/* 정리 작업을 수행합니다. */
			}
		}

		/// <summary>
		/// finalizer
		/// </summary>
		~SapStream()
		{
			this.Dispose(false);
		}

		#endregion IDisposable implementation
	}
}