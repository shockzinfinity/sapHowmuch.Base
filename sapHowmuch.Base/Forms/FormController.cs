using sapHowmuch.Base.Helpers;
using System;

namespace sapHowmuch.Base.Forms
{
	public abstract partial class FormController
	{
		private SAPbouiCOM.IForm _form;

		public SAPbouiCOM.IForm Form
		{
			get
			{
				try
				{
					var dummy = _form?.VisibleEx;
				}
				catch
				{
					_form = null;
				}

				return _form;
			}
			set
			{
				_form = value;
			}
		}

		public int? TypeCount { get { return _form?.TypeCount; } }

		public string TypeEx { get { return _form?.TypeEx; } }

		public string UniqueId { get { return _form?.UniqueID; } }

		public FormController(bool autoStart = false)
		{
			if (autoStart)
				Start();
		}

		~FormController()
		{
			sapHowmuchLogger.Debug("Called FormController.Destruct");
		}

		public virtual void Start()
		{
			if (Form != null)
			{
				Form.Select();
				return;
			}

			try
			{
				var assembly = GetType().Assembly;
				Form = FormHelper.CreateFormFromResource(FormResource, FormType, null, assembly);
				sapHowmuchLogger.Debug($"Form created: Type = {Form.TypeEx}, UID = {Form.UniqueID}");

				try
				{
					MakeFormStream();
					//SubscribeToStream();
				}
				catch (Exception ex)
				{
					SapStream.UiApp.MessageBox($"BindFormEvents Error: {ex.Message}");
				}

				try
				{
					FormCreated();
				}
				catch (Exception ex)
				{
					SapStream.UiApp.MessageBox($"FormCreated Error: {ex.Message}");
				}
			}
			catch (Exception ex)
			{
				SapStream.UiApp.MessageBox($"Failed to open form {FormType}: {ex.Message}");
			}
		}

		public void Close()
		{
			// TODO: dispose 정책 필요
			Form.Close();
			Form = null;
		}

		protected virtual void FormCreated()
		{
		}

		protected virtual void OnFormClosed()
		{
		}

		/// <summary>
		/// view resource
		/// </summary>
		public virtual string FormResource => $"Views.{GetType().Name.Replace("Controller", string.Empty)}.srf";

		public virtual string FormType => $"{GetType().Name.Replace("Controller", string.Empty)}";
		public virtual bool Unique => true;
	}
}