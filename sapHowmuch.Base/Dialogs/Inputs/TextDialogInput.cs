namespace sapHowmuch.Base.Dialogs.Inputs
{
	public class TextDialogInput : IDialogInput
	{
		private readonly string _id;
		private string _defaultValue;
		private bool _required;
		private string _title;
		private SAPbouiCOM.Item _item;
		private SAPbouiCOM.EditText _editText;
		private int _length;

		public TextDialogInput(string id, string title, string defaultValue = null, bool required = false, int length = 20)
		{
			_id = id;
			_title = title;
			_defaultValue = defaultValue;
			_required = required;
			_length = length;
		}

		public string Id => _id;

		public bool Required => _required;

		public string Title => _title;

		public SAPbouiCOM.Item Item
		{
			set
			{
				_item = value;
				_editText = _item.Specific as SAPbouiCOM.EditText;
				_editText.Value = _defaultValue;
				_editText.DataBind.SetBound(true, "", _id);
			}
		}

		public SAPbouiCOM.BoFormItemTypes ItemType => SAPbouiCOM.BoFormItemTypes.it_EDIT;

		public bool Validated
		{
			get
			{
				if (string.IsNullOrEmpty(_editText.Value) && _required)
					return false;

				return true;
			}
		}

		public SAPbouiCOM.BoDataType DataType => SAPbouiCOM.BoDataType.dt_LONG_TEXT;

		public int Length => _length;

		public string DefaultValue => _defaultValue;

		public object GetValue()
		{
			return _editText.Value;
		}

		public void Extras(SAPbouiCOM.Form form, int yPos)
		{
		}
	}
}