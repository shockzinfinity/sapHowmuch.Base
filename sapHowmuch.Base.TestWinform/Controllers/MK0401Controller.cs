using sapHowmuch.Base.Attributes;
using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Enums;
using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using sapHowmuch.Base.Helpers;
using System.Reactive.Linq;

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

				//SapStream.UiApp.Forms.Item(Form.UDFFormUID).VisibleEx = true;
				// NOTE:
				// UDF 폼의 경우 (좌측, 하단, 우측 등의 추가 화면), 화면이 생성될때 UI API 에서 제어한다.
				// 메뉴가 체크되어 있을 경우, 자동으로 VisibleEx 를 조정한다.
				// 생성시에는 임의의 UID 가 생성되며,
				// UDF 폼의 FormTypeEx 에는 부모 폼의 FormTypeEx 에 - 를 prefix 해서 FormTypeEx 에 들어간다.
				// 그리고, 그 해당 UID 는 부모폼의 UDFFormUID 에 저장된다.
				sapHowmuchLogger.Trace($"UDF menu check state: {SapStream.UiApp.Menus.Item(SboMenuItem.ViewUserFields).Checked}");
				sapHowmuchLogger.Trace($"UDFFormUID: {Form.UDFFormUID}");
				sapHowmuchLogger.Trace($"UDFFormUID Visible: {SapStream.UiApp.Forms.Item(Form.UDFFormUID).VisibleEx}");
				sapHowmuchLogger.Trace($"FormSettings.MatrixUID: {Form.Settings.MatrixUID}");

				//_udDocEntry = Form.DataSources.UserDataSources.Add("docEntry", SAPbouiCOM.BoDataType.dt_SHORT_TEXT);
				//_txtDocEntry.DataBind.SetBound(true, string.Empty, _udDocEntry.UID);
				//_udDocEntry.ValueEx = "1";

				SAPbouiCOM.Conditions itemCons = _cflItem.GetConditions();
				SAPbouiCOM.Condition itemCon = itemCons.Add();
				itemCon.Alias = "ItemType";
				itemCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
				itemCon.CondVal = "F"; // fixed asset
				_cflItem.SetConditions(itemCons);

				SAPbouiCOM.Conditions cons = SapStream.UiApp.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_Conditions) as SAPbouiCOM.Conditions;
				SAPbouiCOM.Condition con = cons.Add();

				con.Alias = "DocEntry";
				con.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
				con.CondVal = "1";

				_dbZI0401H.Query(cons);
				_dbZI0401L.Query(cons);

				_matLine.LoadFromDataSourceEx(false);

				Form.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;

				//ItemEventStream.Where(x =>x.DetailArg.EventType == SAPbouiCOM.BoEventTypes.da)

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