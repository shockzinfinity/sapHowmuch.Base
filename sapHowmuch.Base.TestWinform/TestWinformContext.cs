using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Helpers;
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

				// 메뉴로딩테스트
				MenuHelper.LoadFromXML(addonAssembly);

				// 메뉴 스트럭쳐 구성
				//MenuHelper.AddFolder("부가세1", TestConstants.VATRootMenuId1, SboMenuItem.Modules);
				//MenuHelper.AddFolder("부가세2", TestConstants.VATRootMenuId2, SboMenuItem.Modules);

				//MenuHelper.LoadAndAddMenuItemsFromFormControllers(addonAssembly);

				sapHowmuchLogger.Trace("TestWinformContext loaded.");
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
			var form = FormHelper.CreateFormFromResource("Views.TestForm.srf", "fType1", "fId1");

			form.Visible = true;


			var form2 = FormHelper.CreateFormFromResource("Views.TestForm.srf", "fType1", "fId3");

			form2.Visible = true;
		}
	}
}