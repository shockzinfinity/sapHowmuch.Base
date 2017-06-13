using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
