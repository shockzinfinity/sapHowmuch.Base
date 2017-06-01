﻿using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using System;
using System.Collections.Generic;

namespace sapHowmuch.Base.TestWinform.Controllers
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
				Form.VisibleEx = true;
			}
		}

		public override bool Unique => true;

		public string MenuItemTitle => "Test sap form";

		public string[] ParentMenuItemId => new string[] { SboMenuItem.Inventory };

		public IEnumerable<FormMenuItem> MenuItems => new[]
		{
			new FormMenuItem
			{
				MenuItemTitle = "Test sap form",
				ParentMenuItemId =SboMenuItem.Inventory,
			}
		};
	}
}