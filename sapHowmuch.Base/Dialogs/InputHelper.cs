using sapHowmuch.Base.Dialogs.Inputs;
using sapHowmuch.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace sapHowmuch.Base.Dialogs
{
	public class InputHelper
	{
		public const string FormType = "sapHowmuch_Dlg";

		private SAPbouiCOM.Form _form;
		private int _yPos;
		private readonly List<IDialogInput> _dialogInputs = new List<IDialogInput>();
		private bool _canceled;
		private ManualResetEvent _formWait = new ManualResetEvent(false); // form lock 에 대한 문제 해결을 위해...
		private string _infoText;

		public InputHelper(string title, params IDialogInput[] dialogs)
		{
			var formCreator = SapStream.UiApp.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams) as SAPbouiCOM.FormCreationParams;
			formCreator.FormType = FormType;
			formCreator.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
			formCreator.Modality = SAPbouiCOM.BoFormModality.fm_Modal;

			_form = SapStream.UiApp.Forms.AddEx(formCreator);
			_form.Title = title;
			_form.Height = 500;
			_form.Width = 300;
			_yPos = 5;

			if (dialogs != null)
				_dialogInputs.AddRange(dialogs);
		}

		public static InputHelper GetInputs(string title, params IDialogInput[] dialogs)
		{
			return new InputHelper(title, dialogs);
		}

		public InputHelper AddInput(IDialogInput input)
		{
			_dialogInputs.Add(input);
			return this;
		}

		public InputHelper SetInfoText(string text)
		{
			_infoText = text;
			return this;
		}

		public IDictionary<string, object> Result()
		{
			_form.Height = 100 + (_dialogInputs.Count * 15);

			if (string.IsNullOrWhiteSpace(_infoText))
			{
				_yPos += 15;

				var titleText = _form.Items.Add("SH", SAPbouiCOM.BoFormItemTypes.it_STATIC).Specific as SAPbouiCOM.StaticText;
				titleText.Item.Top = _yPos;
				titleText.Item.Left = 10;
				titleText.Item.Width = 250;
				titleText.Caption = _infoText;

				_yPos += 15;
			}

			foreach (var dialogInput in _dialogInputs)
			{
				_yPos += 15;

				// caption
				var titleText = _form.Items.Add($"T{dialogInput.Id}", SAPbouiCOM.BoFormItemTypes.it_STATIC).Specific as SAPbouiCOM.StaticText;
				titleText.Item.Top = _yPos;
				titleText.Item.Left = 10;
				titleText.Item.Width = 150;
				titleText.Caption = dialogInput.Title;

				// datasource
				_form.DataSources.UserDataSources.Add(dialogInput.Id, dialogInput.DataType, dialogInput.Length);

				if (dialogInput.DefaultValue != null)
					_form.DataSources.UserDataSources.Item(dialogInput.Id).ValueEx = dialogInput.DefaultValue;

				// input
				var item = _form.Items.Add(dialogInput.Id, dialogInput.ItemType);
				item.Top = _yPos;
				item.Left = 150;
				dialogInput.Item = item;

				dialogInput.Extras(_form, _yPos);
			}

			_yPos += 20;

			var okButton = _form.Items.Add("okButton", SAPbouiCOM.BoFormItemTypes.it_BUTTON).Specific as SAPbouiCOM.Button;
			okButton.Caption = "Ok";
			okButton.Item.Top = _yPos;
			okButton.Item.Left = 100;
			_form.DefButton = okButton.Item.UniqueID;

			using (_form.FreezeEx())
			{
				_form.VisibleEx = true;
			}

			//var resultDictionary = _dialogInputs.ToDictionary(dialogInput => dialogInput.Id, dialoginput => dialoginput.GetValue());

			var itemEventSubscribes = SapStream.ItemEventStream.Where(e => e.DetailArg.FormUID == _form.UniqueID && e.DetailArg.FormTypeEx == FormType && !e.DetailArg.BeforeAction && e.DetailArg.EventType == SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD).Subscribe(x =>
		   {
			   _canceled = true;
			   _formWait.Set();
		   });

			_formWait.WaitOne();
			_formWait.Reset();

			try
			{
				if (_canceled)
					throw new DialogCanceledException();

				while (_dialogInputs.Any(d => !d.Validated))
				{
					var invalidInputMessage = "Missing values in: " + string.Join(", ", _dialogInputs.Where(d => !d.Validated).Select(i => i.Title));

					SapStream.UiApp.StatusBar.SetText(invalidInputMessage);

					//_formWait.WaitOne();
					//_formWait.Reset();

					if (_canceled)
						throw new DialogCanceledException();
				}

				var resultDictionary = _dialogInputs.ToDictionary(dialogInput => dialogInput.Id, dialogInput => dialogInput.GetValue());

				_form.Close();

				return resultDictionary;
			}
			finally
			{
				itemEventSubscribes.Dispose();
			}
		}
	}
}