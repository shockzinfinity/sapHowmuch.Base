using System.Collections.Generic;

namespace sapHowmuch.Base.Dialogs.Inputs
{
	public class DropdownDialogInput : IDialogInput
	{
		private string _selected;
		private readonly string _id;
		private readonly bool _withEmpty;
		private readonly string _title;
		private SAPbouiCOM.Item _item;
		private SAPbouiCOM.ComboBox _comboBox;
		private readonly IDictionary<string, string> _options;

		public DropdownDialogInput(string id, string title, IDictionary<string, string> options, string selected = null, bool withEmpty = true)
		{
			_id = id;
			_title = title;
			_options = options;
			_selected = selected;
			_withEmpty = withEmpty;
		}

		public string Id => _id;

		public SAPbouiCOM.Item Item
		{
			set
			{
				_item = value;
				_comboBox = _item.Specific as SAPbouiCOM.ComboBox;
				if (_withEmpty)
					foreach (var option in _options)
					{
						_comboBox.ValidValues.Add(option.Key, option.Value);
					}

				_comboBox.ExpandType = SAPbouiCOM.BoExpandType.et_ValueDescription;
				_comboBox.DataBind.SetBound(true, "", _id);
				if (_selected != null)
					_comboBox.Select(_selected, SAPbouiCOM.BoSearchKey.psk_ByValue);
			}
		}

		public bool Required => false;

		public string Title => _title;

		public SAPbouiCOM.BoFormItemTypes ItemType => SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX;

		public bool Validated => true;

		public SAPbouiCOM.BoDataType DataType => SAPbouiCOM.BoDataType.dt_SHORT_TEXT;

		public int Length => 20;

		public string DefaultValue => null;

		public object GetValue()
		{
			return _comboBox.Selected.Value;
		}

		public void Extras(SAPbouiCOM.Form form, int yPos)
		{
		}
	}
}