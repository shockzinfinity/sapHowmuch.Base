﻿using System;
using System.Globalization;

namespace sapHowmuch.Base.Dialogs.Inputs
{
	public class IntegerDialogInput : IDialogInput
	{
		private readonly string _id;
		private int? _defaultValue;
		private bool _required;
		private string _title;
		private SAPbouiCOM.Item _item;
		private SAPbouiCOM.EditText _editText;

		public IntegerDialogInput(string id, string title, int? defaultValue = null, bool required = false)
		{
			_id = id;
			_title = title;
			_defaultValue = defaultValue;
			_required = required;
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
				if (_defaultValue.HasValue)
					_editText.Value = _defaultValue.Value.ToString();

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

				int intValue = 0;
				if (!int.TryParse(_editText.Value, out intValue))
					return false;

				return true;
			}
		}

		public SAPbouiCOM.BoDataType DataType => SAPbouiCOM.BoDataType.dt_LONG_NUMBER;

		public int Length => 0;

		public string DefaultValue => _defaultValue == null ? null : Convert.ToString(_defaultValue, CultureInfo.InvariantCulture);

		public object GetValue()
		{
			return int.Parse(_editText.Value);
		}

		public void Extras(SAPbouiCOM.Form form, int yPos)
		{
		}
	}
}