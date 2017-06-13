using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using sapHowmuch.Base.Helpers;
using System;
using System.Collections.Generic;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	public class TestFormController : FormController
	{
		protected override void OnFormCreate()
		{
			var comboboxItem = Form.Items.Add("comboT01", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);
			comboboxItem.Visible = true;

			var comboBox = comboboxItem.Specific as SAPbouiCOM.ComboBox;

			comboBox.AddComboBoxValues("SELECT [CardCode], [CardName] FROM [OCRD]");

			using (Form.FreezeEx())
			{
				Form.VisibleEx = true;
			}
		}

		public override bool Unique => true;

		#region derived class dispose implementation

		private bool _disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				// free any other managed objects here.
			}

			// free any unmanaged objects here.

			sapHowmuchLogger.Debug($"{GetType().Name} Dispose is called.");

			_disposed = true;

			base.Dispose(disposing);
		}

		~TestFormController()
		{
			sapHowmuchLogger.Debug($"{GetType().Name} Destruct method is called.");
			Dispose(false);
		}

		#endregion
	}
}