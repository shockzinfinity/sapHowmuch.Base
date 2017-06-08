using System;
using System.IO;
using System.Reflection;

namespace sapHowmuch.Base.Helpers
{
	public static class FormHelper
	{
		/// <summary>
		/// create SAP Business One form from internal resource
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

			// formId 를 가지고 호출 하는 경우, 이는 고유한 id 로 폼을 생성하겠다는 의미
			// TypeCount 는 항상 1이어야 하며
			// TypeEx 는 해당 controller 의 type 으로 가도록 한다.
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
				// 고유 id 없이 폼을 만들어서 실행하는 경우
				// 1. srf 의 지정된 UniqueID, Type(FormType) 을 creationPackage 에 할당해서 폼을 생성시킨다.
				// 2. 위의 두 

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
					formXml = textStreamReader.ReadToEnd(); // srf 로딩시점
				}

				var creationPackage = SapStream.UiApp.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams) as SAPbouiCOM.FormCreationParams;

				creationPackage.FormType = formType;
				creationPackage.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
				creationPackage.XmlData = formXml;

				if (formId != null)
				{
					creationPackage.UniqueID = formId;
				}
				else
				{
					SapStream.UiApp.Forms.get
					// TODO: 현재 sap stream 의 forms 에서 해당 typeex 의 폼을 찾은 이후, count 상의 최고 값을 찾고, +1 해서 부여...
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