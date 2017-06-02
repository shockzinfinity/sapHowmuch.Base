using sapHowmuch.Base.Helpers;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace sapHowmuch.Base
{
	public class SapHowmuchAddonContext : ApplicationContext
	{
		public SapHowmuchAddonContext()
		{
			try
			{
				// setup loading 을 위해...
				// 각 애드온 context 에서 로딩
				//var mainAssembly = Assembly.GetEntryAssembly();

				SapStream.ConnectByUI(
					Environment.GetCommandLineArgs().Length > 1 ?
					Environment.GetCommandLineArgs().GetValue(1).ToString() :
					"");

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

				// TODO: setup 정책 결정 (installer 방법으로 가는지, 애드온에 같이 녹이는지...)

				// TODO: 각종 초기화 (특히 UI 관련(e.g. menu, resource 등))

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