using sapHowmuch.Base.EventArguments;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reactive.Linq;
using System.Runtime.InteropServices;

namespace sapHowmuch.Base
{
	public sealed class SapStream : IDisposable
	{
		private static SAPbobsCOM.Company _company; // di company
		private static SAPbouiCOM.Application _application; // ui application

		/// <summary>
		/// SAP DI Company instance
		/// </summary>
		public static SAPbobsCOM.Company DICompany
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
		public static bool IsConnectToUI { get { return _application != null; } }

		/// <summary>
		/// SAP Business One UI API
		/// </summary>
		public static SAPbouiCOM.Application UiApp { get { return _application; } }

		#region various SAP connection methods

		// DI 를 통한 직접 연결
		private static int ConnectDI()
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

		private static bool InitializeSboApplication()
		{
			try
			{
				if (_application != null)
				{
					// app event 등록

					SapStream.AppEventStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_AppEventEventHandler, SapEventArgs<SAPbouiCOM.BoAppEventTypes>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_AppEventEventHandler iHandler = (SAPbouiCOM.BoAppEventTypes eventTypes) =>
							{
								SapEventArgs<SAPbouiCOM.BoAppEventTypes> appEventArgs = new SapEventArgs<SAPbouiCOM.BoAppEventTypes>(DateTime.Now, string.Empty, eventTypes, false);
								handler.Invoke(appEventArgs);
							};

							return iHandler;
						},
						handler => _application.AppEvent += handler,
						handler => _application.AppEvent -= handler);

					SapStream.ItemStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_ItemEventEventHandler, SapEventArgs<SAPbouiCOM.ItemEvent>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_ItemEventEventHandler iHandler = (string formUid, ref SAPbouiCOM.ItemEvent pval, out bool bubble) =>
							{
								bubble = true;
								SapEventArgs<SAPbouiCOM.ItemEvent> itemArgs = new SapEventArgs<SAPbouiCOM.ItemEvent>(DateTime.Now, formUid, pval, bubble);
								handler.Invoke(itemArgs);
								bubble = itemArgs.BubbleEvent;
							};

							return iHandler;
						},
						handler => _application.ItemEvent += handler,
						handler => _application.ItemEvent -= handler);

					SapStream.MenuStream = Observable.FromEvent<SAPbouiCOM._IApplicationEvents_MenuEventEventHandler, SapEventArgs<SAPbouiCOM.MenuEvent>>(
						handler =>
						{
							SAPbouiCOM._IApplicationEvents_MenuEventEventHandler iHandler = (ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent) =>
							{
								BubbleEvent = true;
								SapEventArgs<SAPbouiCOM.MenuEvent> menuArgs = new SapEventArgs<SAPbouiCOM.MenuEvent>(DateTime.Now, string.Empty, pVal, BubbleEvent);
								handler.Invoke(menuArgs);
							};

							return iHandler;
						},
						handler => _application.MenuEvent += handler,
						handler => _application.MenuEvent -= handler);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return true;
		}

		// UI 를 통한 연결
		private static int ConnectUI(string connectionString = "")
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

		private static int ConnectUIDI(string connectionString = "")
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
		private static int DerivingConnect()
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

		#endregion

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

		#endregion

		#endregion various SAP connection methods

		#region Observables

		public static IObservable<SapEventArgs<SAPbouiCOM.ItemEvent>> ItemStream;
		public static IObservable<SapEventArgs<SAPbouiCOM.BoAppEventTypes>> AppEventStream;
		public static IObservable<SapEventArgs<SAPbouiCOM.MenuEvent>> MenuStream;

		#endregion

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