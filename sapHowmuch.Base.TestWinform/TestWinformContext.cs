using sapHowmuch.Base.ChangeTracker;
using sapHowmuch.Base.Helpers;
using sapHowmuch.Base.Setup;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace sapHowmuch.Base.TestWinform
{
	internal class TestWinformContext : SapHowmuchAddonContext
	{
		public TestWinformContext() : base()
		{
			try
			{
				sapHowmuchLogger.Trace("TestWinformContext loading...");

				if (!SapStream.IsUiConnected)
					throw new Exception("SAP Business One not connected");

				// 추가 메뉴
				var addonAssembly = Assembly.GetEntryAssembly();
				sapHowmuchLogger.Debug($"Entry Assembly: {addonAssembly.GetName()}");

				// 메뉴로딩
				MenuHelper.LoadFromXML(addonAssembly);
				sapHowmuchLogger.Trace("TestWinformContext loaded.");

				// setting
				SetupManager.RunSetup(new TestSetup());
				sapHowmuchLogger.Trace("setup completed");

				// change tracker
				// required SBO_SP_PostTransactionNotification
				//ChangeTrackerManager.RunSetup();

				// misc setting
				if (SapStream.UiApp.MetadataAutoRefresh)
				{
					SapStream.UiApp.MetadataAutoRefresh = false;
					sapHowmuchLogger.Trace("Metadata auto refresh off");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}{Environment.NewLine}Exiting...");
				//Application.Exit();
				Environment.Exit(0);
			}
		}

		private void Test_Form()
		{
			// form 직접 생성
			var form = FormHelper.CreateFormFromResource("Views.TestForm.srf", "fType1", "fId1");
			form.VisibleEx = true;

			// form 직접 생성
			var form2 = FormHelper.CreateFormFromResource("Views.TestForm.srf", "fType1", "fId3");
			form2.VisibleEx = true;
		}
	}
}