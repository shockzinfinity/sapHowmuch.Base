using sapHowmuch.Base.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace sapHowmuch.Base.Helpers
{
	public static class FormHelper
	{
		/// <summary>
		/// create SAP Business One form from internal resource
		/// if form is opened using specific unique id, formId parameter must be provided.
		/// </summary>
		/// <param name="resourceName"></param>
		/// <param name="formType"></param>
		/// <param name="formId"></param>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static SAPbouiCOM.IForm CreateFormFromResource(string resourceName, string formType, string formId = null, Assembly assembly = null)
		{
			if (assembly == null)
				assembly = Assembly.GetCallingAssembly();

			if (formId != null)
			{
				try
				{
					// try get existing form
					var form = SapStream.UiApp.Forms.Item(formId);

					return form;
				}
				catch
				{
					// ignored
				}
			}

			try
			{
				string formXml;

				resourceName = string.Concat(assembly.GetName().Name, ".", resourceName);
				var stream = assembly.GetManifestResourceStream(resourceName);
				if (stream == null)
				{
					var embededResources = string.Join(", ", assembly.GetManifestResourceNames());

					throw new Exception($"Failed to load embeded resource '{resourceName}' from Assembly '{assembly.GetName().Name}'. Available Resources: {embededResources}");
				}

				using (var textStreamReader = new StreamReader(stream))
				{
					formXml = textStreamReader.ReadToEnd();
				}

				var creationPackage = SapStream.UiApp.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams) as SAPbouiCOM.FormCreationParams;

				// NOTE: creationPackage 의 formType 의 정보가 우선시 된다.
				// creationPackage.FormType 을 지정하지 않으면 srf 의 form type 으로 셋팅 된다.
				// 고로, FormType 은 FormController 에서 지정하도록 하고, srf에서는 지정하지 않도록 한다.
				creationPackage.FormType = formType;
				creationPackage.XmlData = formXml;

				// TODO: 폼 초기화 정책에 대한 결정 필요
				// TODO: 유저필드 창에 대한 정책 결정 필요
				// TODO: 모달 테스트
				// TODO: 시스템 및 UDO 화면 정책 필요
				//creationPackage.Modality = SAPbouiCOM.BoFormModality.fm_Modal;
				//creationPackage.ObjectType = "";

				if (formId != null)
				{
					creationPackage.UniqueID = formId;
				}
				else
				{
					if (SapStream.UiApp.Forms.AsEnumerable().Any(x => x.TypeEx == formType))
					{
						var maxCount = SapStream.UiApp.Forms.AsEnumerable().Where(x => x.TypeEx == formType).Max(x => x.TypeCount);
						var currentCount = maxCount + 1;
						creationPackage.UniqueID = string.Format($"{formType}_{currentCount}");
					}
					else
					{
						creationPackage.UniqueID = string.Format($"{formType}_1");
					}
				}

				var form = SapStream.UiApp.Forms.AddEx(creationPackage);

				return form;
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to create form from resource {resourceName}: {ex.Message}");
			}
		}
	}
}