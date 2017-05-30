using System;

namespace sapHowmuch.Base.Dialogs.Inputs
{
	public class CheckboxDialogInput : IDialogInput
	{
		private bool _defaultValue;
		private readonly string _id;
		private bool _required;
		private string _title;
		private SAPbouiCOM.Item _item;
		private SAPbouiCOM.CheckBox _checkbox;

		public CheckboxDialogInput(string id, string title, bool defaultValue = false)
		{
			_id = id;
			_title = title;
			_defaultValue = defaultValue;
		}

		public SAPbouiCOM.BoDataType DataType => SAPbouiCOM.BoDataType.dt_SHORT_TEXT;

		public string DefaultValue => null;

		public string Id => _id;

		public SAPbouiCOM.Item Item
		{
			set
			{
				_item = value;
				_checkbox = _item.Specific as SAPbouiCOM.CheckBox;
				_checkbox.DataBind.SetBound(true, string.Empty, _id);
				_checkbox.Checked = _defaultValue;
			}
		}

		public SAPbouiCOM.BoFormItemTypes ItemType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int Length => 1;

		public bool Required => _required;

		public string Title => _title;

		public bool Validated => true;

		public void Extras(SAPbouiCOM.Form form, int yPos)
		{
		}

		public object GetValue()
		{
			return _checkbox.Checked;
		}
	}
}