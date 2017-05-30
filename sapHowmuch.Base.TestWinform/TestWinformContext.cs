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
				if (!SapStream.IsUiConnected)
					throw new Exception("SAP Business One not connected");

				// 화면 띄우는 로직
				Test_Form();
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