using System;
using System.Configuration;
using System.Globalization;
using System.Runtime.InteropServices;

namespace sapHowmuch.Base
{
	public sealed class SapStream : IDisposable
	{
		private static SAPbobsCOM.Company _company; // di company
		private static SAPbouiCOM.Application _application; // ui application

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

			//ConfigurationManager.AppSettings["sapDBType"];
			//ConfigurationManager.AppSettings["sapDBUserName"];
			//ConfigurationManager.AppSettings["sapDBPassword"];

			try
			{
				retVal = _company.Connect();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

			return retVal;
		}

		// UI 를 통한 연결
		private static int ConnectUI(string connectionString = "")
		{
			int retVal = 0;

			if (string.IsNullOrWhiteSpace(connectionString))
			{
				// DEBUG 일 경우
				connectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";
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

		#endregion
	}
}