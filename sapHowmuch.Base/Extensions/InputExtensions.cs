using sapHowmuch.Base.Dialogs;
using sapHowmuch.Base.Dialogs.Inputs;
using System;

namespace sapHowmuch.Base.Extensions
{
	public static class InputExtensions
	{
		public static InputHelper AddText(this InputHelper inputHelper, string id, string title, string defaultValue = null, bool required = false, int length = 20)
		{
			return inputHelper.AddInput(new TextDialogInput(id, title, defaultValue, required, length));
		}

		public static void CreateDirButton(this SAPbouiCOM.Form form, string id, int yPos, Action action)
		{
			var dirButton = form.Items.Add($"{id}E", SAPbouiCOM.BoFormItemTypes.it_BUTTON);

			dirButton.Top = yPos + 2;
			dirButton.Left = 235;
			dirButton.Height = 11;
			dirButton.Width = 13;

			var dirButtonSpec = dirButton.Specific as SAPbouiCOM.Button;
			dirButtonSpec.Caption = "...";
			dirButtonSpec.ClickAfter += (sboObject, val) => action();
		}
	}
}