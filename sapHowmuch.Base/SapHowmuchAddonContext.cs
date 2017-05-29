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
				// TODO: setup 정책 결정 (installer 방법으로 가는지, 애드온에 같이 녹이는지...)
				var mainAssembly = Assembly.GetEntryAssembly();

				SapStream.ConnectByUI(
					Environment.GetCommandLineArgs().Length > 1 ?
					Environment.GetCommandLineArgs().GetValue(1).ToString() :
					"");

				// TODO: 각종 초기화 (특히 UI 관련(e.g. menu, resource 등))
			}
			catch (Exception ex)
			{
				MessageBox.Show($"SAP Business One error: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}Exiting...");
				ExitThread();
			}
		}
	}
}