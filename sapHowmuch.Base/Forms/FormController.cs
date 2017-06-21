using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Helpers;
using System;

namespace sapHowmuch.Base.Forms
{
	public abstract partial class FormController : IDisposable
	{
		private SAPbouiCOM.IForm _form;

		/// <summary>
		/// SAP Business One UI API form object
		/// </summary>
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

		/// <summary>
		/// SAP Business One UI API type count getter
		/// </summary>
		public int? SboTypeCount { get { return _form?.TypeCount; } }

		/// <summary>
		/// SAP Business One UI API type value getter
		/// </summary>
		public string SboTypeEx { get { return _form?.TypeEx; } }

		/// <summary>
		/// SAP Business One UI API unique id getter
		/// </summary>
		public string UniqueId { get { return _form?.UniqueID; } }

		/// <summary>
		/// view resource
		/// </summary>
		public virtual string FormResource => $"Views.{GetType().Name.Replace("Controller", string.Empty)}.srf";

		/// <summary>
		/// Form Type
		/// </summary>
		public virtual string FormType => $"{GetType().Name.Replace("Controller", string.Empty)}";

		/// <summary>
		/// Is Unique in SAP Business One UI API Forms
		/// </summary>
		public virtual bool Unique => true;

		public FormController(bool autoStart = false)
		{
			if (autoStart)
				Start();
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
				sapHowmuchLogger.Debug($"Form created: Type = {Form.TypeEx}, UID = {Form.UniqueID}, ObjectType = {Form.BusinessObject.Type}");

				try
				{
					// srf 컨트롤 할당
					this.AssignSrfControlTo();
					// event stream
					MakeFormStream();
				}
				catch (Exception ex)
				{
					SapStream.UiApp.MessageBox($"Form initializing Error: {ex.Message}");
				}

				try
				{
					OnFormCreate();
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

		protected virtual void OnFormCreate()
		{
		}

		public void Close()
		{
			this.DisposeSrfControlFrom();
			this.Form.Close();
			this.Form.ReleaseComObject();
			this.Form = null;
		}

		#region IDisposable implementation

		private bool _disposed = false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			// TODO: dispose 테스트 필요
			// 각 메서드 내부에서 사용한 COM object 에 대한 명시적인 메모리 해제가 과연 의미가 있는가?
			if (_disposed) return;

			if (disposing)
			{
				// free any other manaaged objects here.
			}

			// free any unmanaged objects here.
			// specify srf control placeholder

			sapHowmuchLogger.Debug("Called FormController.Dispose");

			_disposed = true;
		}

		~FormController()
		{
			sapHowmuchLogger.Debug("Called FormController.Destruct");
			Dispose(false);
		}

		#endregion IDisposable implementation
	}
}