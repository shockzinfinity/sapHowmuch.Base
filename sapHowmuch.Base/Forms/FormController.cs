using sapHowmuch.Base.Helpers;
using System;

namespace sapHowmuch.Base.Forms
{
	public abstract class FormController
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

		public FormController(bool autoStart = false)
		{
			if (autoStart)
				Start();
		}

		~FormController()
		{
			sapHowmuchLogger.Debug("Called FormController.Destruct");
		}

		public void Start()
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
				sapHowmuchLogger.Debug($"Form created: Type = {Form.Type}, UID = {Form.UniqueID}");

				try
				{
					FormCreated();
				}
				catch (Exception ex)
				{
					SapStream.UiApp.MessageBox($"FormCreated Error: {ex.Message}");
				}

				try
				{
					BindFormEvents();
				}
				catch (Exception ex)
				{
					SapStream.UiApp.MessageBox($"BindFormEvents Error: {ex.Message}");
				}
			}
			catch (Exception ex)
			{
				SapStream.UiApp.MessageBox($"Failed to open form {FormType}: {ex.Message}");
			}
		}

		public void Close()
		{
			Form.Close();
			Form = null;
		}

		public virtual void FormCreated()
		{
		}

		public virtual void BindFormEvents()
		{
		}

		/// <summary>
		/// view resource
		/// TODO: b1s 파일 지원
		/// </summary>
		public virtual string FormResource => $"Views.{GetType().Name.Replace("Controller", string.Empty)}.srf";

		public virtual string FormType => $"{GetType().Name.Replace("Controller", string.Empty)}";
		public virtual bool Unique => true;
	}
}