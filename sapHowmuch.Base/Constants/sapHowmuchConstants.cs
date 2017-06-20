#pragma warning disable 1591

namespace sapHowmuch.Base
{
	public static class sapHowmuchConstants
	{
		public const string SapUiAppName = "SAP Business One.exe";
		public const string SapUiDebugConnectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";
		public const string SapTempAddonIdentifier = "5645523035496D706C656D656E746174696F6E3A5330343134333233313438D97E42D4155C03AF691C13952E1B7EA4BA9FE8CC";

		#region setting services

		public const string SettingTableName = "SHM_SETTING";
		public const string SettingTableDescription = "sapHowmuch Addon setting table";
		public const string SettingFieldName = "SettingValue";
		public const string SettingFieldDescription = "Setting Value";

		#endregion setting services

		#region menu attribute constants

		public const string MenuFormTypeAttr = "FormType";
		public const string MenuUniqueIdAttr = "UniqueID";
		public const string MenuFatherUIDAttr = "FatherUID";
		public const string MenuPositionAttr = "Position";
		public const string MenuTitleAttr = "String";
		public const string MenuTypeAttr = "Type";
		public const string MenuThreadedActionAttr = "ThreadedAction";
		public const string MenuImageAttr = "Image";

		#endregion menu attribute constants

		#region change Tracker constants

		public const string CT_UdtName = "SHM_CHANGETRACKER";
		public const string CT_UdfKey = "SHM_CT_Key";
		public const string CT_UdfObj = "SHM_CT_Obj";

		#endregion change Tracker constants

		#region ui controls

		public const string ComboboxWholeDescription = "전체";

		#endregion
	}
}