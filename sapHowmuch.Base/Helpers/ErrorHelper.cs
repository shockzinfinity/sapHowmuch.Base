using System;
using System.Diagnostics;

namespace sapHowmuch.Base.Helpers
{
	/// <summary>
	/// SAP Business One Error Helper
	/// </summary>
	public class ErrorHelper
	{
		/// <summary>
		/// Get Last Error Message from SAP Business One DI API
		/// </summary>
		/// <returns></returns>
		public static SboError GetLastErrorMessage()
		{
			int errorCode;
			string errorMessage;

			SapStream.DICompany.GetLastError(out errorCode, out errorMessage);
			return new SboError(errorCode, errorMessage);
		}

		/// <summary>
		/// Handle Return Code
		/// Throws Exception if Return Code != 0
		/// </summary>
		/// <param name="returnCode"></param>
		/// <param name="errorDescription"></param>
		public static void HandleErrorWithException(int returnCode, string errorDescription)
		{
			if (returnCode == 0)
				return;

			var error = GetLastErrorMessage();
			throw new Exception($"{errorDescription}: {error.Code} {error.Message}");
		}

		/// <summary>
		/// Handle Return Code through Ui Api
		/// </summary>
		/// <param name="returnCode"></param>
		/// <param name="errorDescription"></param>
		/// <returns></returns>
		public static bool HandleErrorWithMessageBox(int returnCode, string errorDescription = "Error")
		{
			if (returnCode == 0)
				return true;

			var error = GetLastErrorMessage();
			var errorMessage = $"{errorDescription}: {error.Code} {error.Message}";
			SapStream.UiApp.StatusBar.SetText(errorMessage, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
			SapStream.UiApp.MessageBox(errorMessage);

			return false;
		}
	}

	[DebuggerDisplay("{Code}: {Message}")]
	public class SboError
	{
		public readonly int Code;
		public readonly string Message;

		public SboError(int errorCode, string errorMessage)
		{
			this.Code = errorCode;
			this.Message = errorMessage;
		}
	}
}