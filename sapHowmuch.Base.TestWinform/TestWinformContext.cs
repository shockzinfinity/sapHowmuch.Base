using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Helpers;
using System;
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
				MenuHelper.AddFolder("부가세 관련", TestConstants.VATRootMenuId, SboMenuItem.Finance);

				// 화면 띄우는 로직
				//Test_Form();

				sapHowmuchLogger.Trace("TestWinformContext loaded.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}{Environment.NewLine}Exiting...");
				Application.Exit();
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