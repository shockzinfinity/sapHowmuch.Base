using sapHowmuch.Base.Attributes;
using sapHowmuch.Base.Enums;
using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using sapHowmuch.Base.Helpers;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	internal class MK0401Controller : FormController
	{
		#region general form settings

		public override bool Unique => false;

		#endregion general form settings

		protected override void OnFormCreate()
		{
			using (Form.FreezeEx())
			{
				Form.DataBrowser.BrowseBy = _txtDocEntry.Item.UniqueID;
				Form.DefButton = _btnOk.Item.UniqueID;

				string periodQuery = "SELECT [AbsEntry], [PeriodCat] FROM [OACP] ORDER BY [PeriodCat] DESC";
				_cmbYear.ValidValues.Query(periodQuery, true);
				_cmbYear.Select(_cmbYear.ValidValues.Count - 1, SAPbouiCOM.BoSearchKey.psk_Index);

				string userQuery = "SELECT [empID], [LastName] + [FirstName] FROM [OHEM]";
				string deptQuery = "SELECT [Code], [Name] FROM [OUDP]";
				string queryUserProp = "SELECT [U_Minor], [U_CdName] FROM [@AD0020L] WHERE [Code] = 'Z131'";
				string queryTypeCode = "SELECT [U_Minor], [U_CdName] FROM [@AD0020L] WHERE [Code] = 'Z132'";
				_matLine.Columns.Item("Col_2").ValidValues.Query(userQuery);
				_matLine.Columns.Item("Col_3").ValidValues.Query(deptQuery);
				_matLine.Columns.Item("Col_4").ValidValues.Query(queryUserProp);
				_matLine.Columns.Item("Col_5").ValidValues.Query(queryTypeCode);
				_matLine.Columns.Item("Col_8").ValidValues.Query(queryTypeCode);

				Form.VisibleEx = true;
			}

			sapHowmuchLogger.Trace($"{this.Form.Title} // UID: {this.UniqueId} // [{this.FormResource}]");
		}

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

		~MK0401Controller()
		{
			sapHowmuchLogger.Debug($"{GetType().Name} Destruct method is called.");
			Dispose(false);
		}

		#endregion derived class dispose implementation

		#region control placeholders

		// TODO: code snippet 으로 변경 필요
		[SrfControl("@ZI0401H", SrfControlType.DBDataSource)]
		private SAPbouiCOM.DBDataSource _dbZI0401H;

		[SrfControl("@ZI0401L", SrfControlType.DBDataSource)]
		private SAPbouiCOM.DBDataSource _dbZI0401L;

		[SrfControl("Item_1", SrfControlType.ComboBox)]
		private SAPbouiCOM.ComboBox _cmbYear;

		[SrfControl("Item_2", SrfControlType.EditText)]
		private SAPbouiCOM.EditText _txtFACode;

		[SrfControl("Item_5", SrfControlType.EditText)]
		private SAPbouiCOM.EditText _txtFAName;

		[SrfControl("Item_15", SrfControlType.EditText)]
		private SAPbouiCOM.EditText _txtDepreciationFrom;

		[SrfControl("Item_17", SrfControlType.EditText)]
		private SAPbouiCOM.EditText _txtDepreciationTo;

		[SrfControl("Item_23", SrfControlType.EditText)]
		private SAPbouiCOM.EditText _txtCarModel;

		[SrfControl("Item_24", SrfControlType.EditText)]
		private SAPbouiCOM.EditText _txtCarNumber;

		[SrfControl("Item_10", SrfControlType.EditText)]
		private SAPbouiCOM.EditText _txtDocEntry;

		[SrfControl("Item_12", SrfControlType.EditText)]
		private SAPbouiCOM.EditText _txtDocDate;

		[SrfControl("Item_14", SrfControlType.EditText)]
		private SAPbouiCOM.EditText _txtUpdateDate;

		[SrfControl("1", SrfControlType.Button)]
		private SAPbouiCOM.Button _btnOk;

		[SrfControl("Item_6", SrfControlType.Button)]
		private SAPbouiCOM.Button _btnExcelUpload;

		[SrfControl("Item_36", SrfControlType.Button)]
		private SAPbouiCOM.Button _btnExcelDownload;

		[SrfControl("Item_4", SrfControlType.Matrix)]
		private SAPbouiCOM.Matrix _matLine;

		[SrfControl("CFL_0", SrfControlType.ChooseFromList)]
		private SAPbouiCOM.ChooseFromList _cflItem;

		#endregion control placeholders
	}
}