namespace sapHowmuch.Base.Enums
{
	/// <summary>
	/// Hash Type enum
	/// </summary>
	public enum HashType
	{
		HMAC, HMACMD5, HMACSHA1, HMACSHA256, HMACSHA384, HMACSHA512,
		MACTripleDES, MD5, RIPEMD160, SHA1, SHA256, SHA384, SHA512
	}

	public enum DateTimeInterval
	{
		Year,
		Month,
		Day,
		Hour,
		Minute,
		Second,
		MiliSecond
	}

	/// <summary>
	/// UI Control type of SAPbouiCOM
	/// </summary>
	public enum SrfControlType
	{
		None = 0,
		Button = 1,
		StaticText,
		EditText,
		Folder,
		Rectangle,
		ActiveX,
		PaneComboBox,
		ComboBox,
		LinkedButton,
		PictureBox,
		ExtendedEditText,
		CheckBox,
		OptionBtn,
		Matrix,
		Grid,
		ButtonCombo,
		WebBrowser,
		DBDataSource,
		UserDataSource,
		DataTable,
		ChooseFromList
	}

	public enum AddonFileType
	{
		InstallFile,
		ReportFile,
		ExcelFile
	}

	public class AddonClientType
	{
		public static string Windows = "W";
		public static string All = "A";
		public static string Browser = "B";
	}

	public class AddonGroupType
	{
		public static string Mandatory = "C";
		public static string NoMandatory = "M";
	}

	public class AddonPlatform
	{
		public static string x86 = "N";
		public static string x64 = "X";
	}
}