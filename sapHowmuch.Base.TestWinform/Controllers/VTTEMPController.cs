using sapHowmuch.Base.Attributes;
using sapHowmuch.Base.Enums;
using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using sapHowmuch.Base.Helpers;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	internal class VTTEMPController : FormController
	{
		protected override void OnFormCreate()
		{
			using (Form.FreezeEx())
			{
				this.Form.Title = $"{this.Form.Title} // UID: {this.UniqueId}";

				Form.VisibleEx = true;

				//ItemEventStream.Where(x => x.DetailArg.ItemUID == _firstEditText.Item.UniqueID).Subscribe(ev =>
				//{
				//	// for test
				//	//sapHowmuchLogger.Trace($"{ev.DetailArg.ItemUID}'s value is :{_firstEditText.Value}");
				//});
			}
		}

		public override bool Unique => false;

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

		~VTTEMPController()
		{
			sapHowmuchLogger.Debug($"{GetType().Name} Destruct method is called.");
			Dispose(false);
		}

		#endregion derived class dispose implementation

		#region placeholders

		[SrfControl("Item_0", SrfControlType.EditText)]
		private SAPbouiCOM.EditText _firstEditText;

		[SrfControl("Item_1", SrfControlType.ComboBox)]
		private SAPbouiCOM.ComboBox _secondCombo;

		[SrfControl("Item_2", SrfControlType.Grid)]
		private SAPbouiCOM.Grid _thirdGrid;

		#endregion placeholders
	}
}