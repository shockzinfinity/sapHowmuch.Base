using sapHowmuch.Base.Forms;
using sapHowmuch.Base.Helpers;
using System.Collections.Generic;

namespace sapHowmuch.Base.Extensions
{
	public static class SboFormExtensions
	{
		public static SAPbouiCOM.Form GetForm(this SAPbouiCOM.SBOItemEventArg pVal)
		{
			return SapStream.UiApp.Forms.Item(pVal.FormUID);
		}

		public static SAPbouiCOM.StaticText GetStaticText(this SAPbouiCOM.IForm form, string itemId)
		{
			return form.Items.Item(itemId).Specific as SAPbouiCOM.StaticText;
		}

		public static SAPbouiCOM.ComboBox GetComboBox(this SAPbouiCOM.IForm form, string itemId)
		{
			return form.Items.Item(itemId).Specific as SAPbouiCOM.ComboBox;
		}

		public static SAPbouiCOM.Button GetButton(this SAPbouiCOM.IForm form, string itemId)
		{
			return form.Items.Item(itemId).Specific as SAPbouiCOM.Button;
		}

		public static SAPbouiCOM.EditText GetEditText(this SAPbouiCOM.IForm form, string itemId)
		{
			return form.Items.Item(itemId).Specific as SAPbouiCOM.EditText;
		}

		public static SAPbouiCOM.CheckBox GetCheckBox(this SAPbouiCOM.IForm form, string itemId)
		{
			return form.Items.Item(itemId).Specific as SAPbouiCOM.CheckBox;
		}

		public static SAPbouiCOM.Matrix GetMatrix(this SAPbouiCOM.IForm form, string itemId)
		{
			return form.Items.Item(itemId).Specific as SAPbouiCOM.Matrix;
		}

		public static SAPbouiCOM.Grid GetGrid(this SAPbouiCOM.IForm form, string itemId)
		{
			return form.Items.Item(itemId).Specific as SAPbouiCOM.Grid;
		}

		public static void AddComboBoxValues(this SAPbouiCOM.ComboBox comboBox, string sql)
		{
			if (comboBox.ValidValues.Count > 0)
			{
				// TODO: 테스트 필요
				var count = comboBox.ValidValues.Count;
				for (int i = count - 1; i >= 0; i--)
				{
					comboBox.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
				}
			}

			using (var query = new SboRecordsetQuery(sql))
			{
				foreach (var combo in query.Result)
				{
					comboBox.ValidValues.Add(combo.Item(0).Value.ToString(), combo.Item(1).Value.ToString());
				}
			}
		}

		public static Freeze FreezeEx(this SAPbouiCOM.IForm form)
		{
			return new Freeze(form);
		}

		public static IEnumerable<SAPbouiCOM.Form> AsEnumerable(this SAPbouiCOM.Forms forms)
		{
			foreach (SAPbouiCOM.Form item in forms)
			{
				yield return item;
			}
		}
	}
}