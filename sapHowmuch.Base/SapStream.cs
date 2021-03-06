﻿using sapHowmuch.Base.Helpers;
using System;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace sapHowmuch.Base
{
	/// <summary>
	/// This provides <c>SapStream</c> class.
	/// </summary>
	public sealed partial class SapStream : IDisposable
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
					ConnectByDIWithConfig();
				}

				return _company;
			}
		}

		/// <summary>
		/// is connected ui app
		/// </summary>
		public static bool IsUiConnected => _application != null && IsDiConnected;

		/// <summary>
		/// is connected di api
		/// </summary>
		public static bool IsDiConnected => _company != null && _company.Connected;

		/// <summary>
		/// SAP Business One UI API
		/// </summary>
		public static SAPbouiCOM.Application UiApp { get { return _application; } }

		#region various SAP connection methods

		// UI 를 통한 연결
		public static void ConnectByUI(string connectionString = "")
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				// if connection string is empty, then use debug mode
				// DEBUG mode
				connectionString = sapHowmuchConstants.SapUiDebugConnectionString;
			}
			else
			{
				// info log
				sapHowmuchLogger.Info($"current connection string: {connectionString}");
			}

			var sbGuiApi = new SAPbouiCOM.SboGuiApi();
			_company = new SAPbobsCOM.Company();

			try
			{
				sbGuiApi.Connect(connectionString);
				_application = sbGuiApi.GetApplication();

				var contextCookie = _company.GetContextCookie();
				var diCompanyConnectionString = _application.Company.GetConnectionContext(contextCookie);
				var responseCode = _company.SetSboLoginContext(diCompanyConnectionString);
				ErrorHelper.HandleErrorWithException(responseCode, "DI api could not set Sbo login context");

				var connectResponse = _company.Connect();
				ErrorHelper.HandleErrorWithException(connectResponse, "DI api could not connect");

				var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
				sapHowmuchLogger.Info($"{assemblyName} connected");

				InitializeSboApplication();
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"SboApp UI connect error: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
				throw ex;
			}
		}

		// DI 를 통한 직접 연결
		public static void ConnectByDI(string server, SAPbobsCOM.BoDataServerTypes dbServerType, string companyDb, string dbUserName, string dbPassword, string userName = null, string password = null, string licenseServer = null, bool connectWithUi = false)
		{
			try
			{
				_company = new SAPbobsCOM.Company
				{
					Server = server,
					DbServerType = dbServerType,
					CompanyDB = companyDb
				};

				if (licenseServer != null)
					_company.LicenseServer = licenseServer;

				if (!string.IsNullOrWhiteSpace(userName))
				{
					_company.UseTrusted = false;
					_company.UserName = userName;
					_company.Password = password;
					_company.DbUserName = dbUserName;
					_company.DbPassword = dbPassword;
				}
				else
				{
					_company.UseTrusted = true;
				}

				int retVal = _company.Connect();

				if (retVal != 0)
				{
					int errorCode;
					string errorMessage;

					_company.GetLastError(out errorCode, out errorMessage);

					sapHowmuchLogger.Debug($"Servername={server}, ServerType={dbServerType}, CompanyDb={companyDb}, DbUsername={dbUserName}, DbPassword={dbPassword}, SboUsername={userName}, SboPassword={password}, LicenceServer={licenseServer}");

					throw new Exception($"DI connection failed: {errorCode} : {errorMessage}");
				}

				if (connectWithUi)
				{
					if (ProcessHelper.ByName(sapHowmuchConstants.SapUiAppName).Count() > 0)
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
						sapHowmuchLogger.Error($"SAP Business One process could not found");
					}
				}
			}
			catch
			{
				throw;
			}
		}

		public static void ConnectByDIWithConfig()
		{
			var server = ConfigurationManager.AppSettings["sapServer"];
			var dbServerType = (SAPbobsCOM.BoDataServerTypes)Enum.Parse(typeof(SAPbobsCOM.BoDataServerTypes), ConfigurationManager.AppSettings["sapDBType"]);
			var companyDb = ConfigurationManager.AppSettings["sapCompanyDBName"];
			var licenseServer = ConfigurationManager.AppSettings["sapLicenseServer"];
			var userName = ConfigurationManager.AppSettings["sapUserName"];
			var password = ConfigurationManager.AppSettings["sapPassword"];
			var dbUserName = ConfigurationManager.AppSettings["sapDbUserName"];
			var dbPassword = ConfigurationManager.AppSettings["sapDbPassword"];

			ConnectByDI(server, dbServerType, companyDb, dbUserName, dbPassword, userName, password, licenseServer);
		}

		// deriving a connection
		//private int DerivingConnect()
		//{
		//	int uidiConnect = ConnectUIDI();
		//	int retVal = 0;

		//	if (uidiConnect == 0)
		//	{
		//		SAPbobsCOM.Company company2 = new SAPbobsCOM.Company();

		//		try
		//		{
		//			var cookie = company2.GetContextCookie();
		//			var context = _application.Company.GetConnectionContext(cookie);
		//			company2.SetSboLoginContext(context);
		//			company2.CompanyDB = "DB_TestConnection2";
		//			retVal = company2.Connect();

		//			if (retVal == 0)
		//			{
		//				Console.WriteLine("Gui connected to {0}", _company.CompanyName);
		//				Console.WriteLine("New connection to {0}", company2.CompanyName);
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			Console.WriteLine(ex.Message);
		//		}
		//		finally
		//		{
		//			Marshal.FinalReleaseComObject(company2);
		//		}
		//	}

		//	return retVal;
		//}

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