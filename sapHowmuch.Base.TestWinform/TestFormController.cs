﻿using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using System;

namespace sapHowmuch.Base.TestWinform
{
	public class TestFormController : FormController, IFormMenuItem
	{
		public override void FormCreated()
		{
			var comboboxItem = Form.Items.Add("comboT01", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);
			comboboxItem.Visible = true;

			var comboBox = comboboxItem.Specific as SAPbouiCOM.ComboBox;

			comboBox.AddComboBoxValues("SELECT [CardCode], [CardName] FROM [OCRD]");

			using (Form.FreezeEx())
			{

			}
		}

		public override bool Unique => false;

		public string MenuItemTitle => "Test sap form";

		public string ParentMenuItemId => SboMenuItem.Inventory;
	}
}