using sapHowmuch.Base.Attributes;
using sapHowmuch.Base.Enums;
using sapHowmuch.Base.Forms;
using sapHowmuch.Base.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace sapHowmuch.Base.Extensions
{
	public static class FormControllerExtensions
	{
		public static IEnumerable<FieldInfo> GetAttributeOfType<T>(this FormController formController) where T : Attribute
		{
			var fieldInfos = formController.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

			foreach (var item in fieldInfos)
			{
				if (item.IsDefined(typeof(T), false))
				{
					yield return item;
				}
			}
		}

		public static Dictionary<FieldInfo, T> GetFieldsWithAttribute<T>(this FormController formController) where T : Attribute
		{
			var fieldInfos = formController.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

			return fieldInfos.Where(x => x.IsDefined(typeof(T), false)).ToDictionary(x => x, x => x.GetCustomAttribute<T>());
		}

		public static void AssignSrfControlTo(this FormController formController)
		{
			// TODO: 성능저하 이슈?
			var fieldsWithAttribute = formController.GetFieldsWithAttribute<SrfControlAttribute>();

			foreach (var item in fieldsWithAttribute.Keys)
			{
				AssignControl(item, formController, fieldsWithAttribute[item]);
			}
		}

		public static void DisposeSrfControlFrom(this FormController formController)
		{
			// srf control dispose
			// formcontroller 는 null 시키지 않고, 각 클래스에서 dispose 구현하도록 교육

			var fieldsWithAttribute = formController.GetFieldsWithAttribute<SrfControlAttribute>();

			foreach (var item in fieldsWithAttribute.Keys)
			{
				item.ReleaseComObject();
				//formController.Form.Items.Item(fieldsWithAttribute[item].UniqueId).ReleaseComObject();
				item.SetValue(formController, null);
			}

			//if (formController.Form != null)
			//{
			//	formController.Form.Close();
			//	formController.Form = null;
			//}
		}

		private static void AssignControl(FieldInfo field, FormController controller, SrfControlAttribute attribute)
		{
			try
			{
				switch (attribute.ControlType)
				{
					case SrfControlType.None:
						field.SetValue(controller, null);
						break;

					case SrfControlType.Button:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.Button));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.StaticText:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.StaticText));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.EditText:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.EditText));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.Folder:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.Folder));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.Rectangle:
						field.SetValue(controller, null);
						break;

					case SrfControlType.ActiveX:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.ActiveX));
						break;

					case SrfControlType.PaneComboBox:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.PaneComboBox));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.ComboBox:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.ComboBox));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.LinkedButton:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.LinkedButton));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.PictureBox:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.PictureBox));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.ExtendedEditText:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.EditText));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.CheckBox:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.CheckBox));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.OptionBtn:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.OptionBtn));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.Matrix:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.Matrix));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.Grid:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.Grid));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.ButtonCombo:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.ButtonCombo));
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						controller.Form.Items.Item(attribute.UniqueId).AffectsFormMode = attribute.AffectsFormMode;
						break;

					case SrfControlType.WebBrowser:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.WebBrowser));
						break;

					case SrfControlType.DBDataSource:
						field.SetValue(controller, (controller.Form.DataSources.DBDataSources.Item(attribute.UniqueId) as SAPbouiCOM.DBDataSource));
						break;

					case SrfControlType.UserDataSource:
						field.SetValue(controller, (controller.Form.DataSources.UserDataSources.Item(attribute.UniqueId) as SAPbouiCOM.UserDataSource));
						break;

					case SrfControlType.DataTable:
						field.SetValue(controller, (controller.Form.DataSources.DataTables.Item(attribute.UniqueId) as SAPbouiCOM.DataTable));
						break;

					case SrfControlType.ChooseFromList:
						field.SetValue(controller, (controller.Form.Items.Item(attribute.UniqueId).Specific as SAPbouiCOM.ChooseFromList));
						break;
				}
			}
			catch(Exception ex)
			{
				sapHowmuchLogger.Error($"Assigning '{attribute.UniqueId}' has failed {ex.Message}");
				//throw ex; // ignore?
			}
		}
	}
}