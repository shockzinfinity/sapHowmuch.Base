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
					formXml = textStreamReader.ReadToEnd(); // srf 로딩시점
				}

				// TODO: srf 파일안의 unique id, type 등의 정보 처리 문제 해결 필요, injection?

				var creationPackage = SapStream.UiApp.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams) as SAPbouiCOM.FormCreationParams;

				// TODO: 그냥 xml 로딩만으로 바뀌는지에 대한 테스트 필요
				creationPackage.FormType = formType;
				creationPackage.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
				creationPackage.XmlData = formXml;

				if (formId != null)
					creationPackage.UniqueID = formId;

				var form = SapStream.UiApp.Forms.AddEx(creationPackage);

				// srf 파일안의 uniqueid, type 이 자동으로 지정되게 됨?
				// add 이후의 값이 변경되는 부분을 체크할 필요가 있음.

				return form;
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to create form from resource {resourceName}: {ex.Message}");
			}
		}
	}
}