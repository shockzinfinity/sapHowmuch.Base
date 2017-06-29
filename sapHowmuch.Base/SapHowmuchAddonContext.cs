using sapHowmuch.Base.Helpers;
using System;
using System.Windows.Forms;

namespace sapHowmuch.Base
{
	public class SapHowmuchAddonContext : ApplicationContext
	{
		public SapHowmuchAddonContext()
		{
			try
			{
				// NOTE: ui api 를 통한 연결은 관리자 권한 검증이 필요하다.
				// 관지라로 실행되지 않았을 경우, ui api 연결에서 connection string 관련 에러가 발생한다.
				// 고로, 프로세스 검증을 통해 관리자로 실행되었는지에 대한 검증이 필요하다.
				// 해당 검증은 SapHowmuchAddonContext 에서 수행한다.
				// TODO: 프로세스 권한 체크

				// setup loading 을 위해...
				// 각 애드온 context 에서 로딩
				//var mainAssembly = Assembly.GetEntryAssembly();

				SapStream.ConnectByUI(
					Environment.GetCommandLineArgs().Length > 1 ?
					Environment.GetCommandLineArgs().GetValue(1).ToString() :
					"");
				//SapStream.ConnectByDIWithConfig(true);

				sapHowmuchLogger.Trace(string.Format("Given connection string: {0}", Environment.GetCommandLineArgs().Length > 1 ? Environment.GetCommandLineArgs().GetValue(1).ToString() : ""));

				sapHowmuchLogger.Trace($"AppId: {SapStream.UiApp.AppId}");
				sapHowmuchLogger.Trace($"MetadataAutoRefresh: {SapStream.UiApp.MetadataAutoRefresh.ToString()}");
				sapHowmuchLogger.Trace($"IsHostedEnvironment: {SapStream.UiApp.IsHostedEnvironment.ToString()}");
				sapHowmuchLogger.Trace($"Language: {SapStream.UiApp.Language.ToString()}");
				sapHowmuchLogger.Trace($"AddonIdentifier: {SapStream.DICompany.AddonIdentifier}");
				sapHowmuchLogger.Trace($"AttachMentPath: {SapStream.DICompany.AttachMentPath}");
				sapHowmuchLogger.Trace($"BitMapPath: {SapStream.DICompany.BitMapPath}");
				sapHowmuchLogger.Trace($"ExcelDocsPath: {SapStream.DICompany.ExcelDocsPath}");
				sapHowmuchLogger.Trace($"WordDocsPath: {SapStream.DICompany.WordDocsPath}");
				sapHowmuchLogger.Trace($"SecurityCode: {SapStream.DICompany.SecurityCode}");
				sapHowmuchLogger.Trace($"UserSignature: {SapStream.DICompany.UserSignature.ToString()}");
				sapHowmuchLogger.Trace($"Version: {SapStream.DICompany.Version.ToString()}");

				// 각 애드온 context 에서 로딩
				//MenuHelper.LoadAndAddMenuItemsFromFormControllers(mainAssembly);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"SAP Business One error: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}Exiting...");
				ExitThread();
			}
		}
	}
}