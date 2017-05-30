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
					formXml = textStreamReader.ReadToEnd();
				}

				var creationPackage = SapStream.UiApp.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams) as SAPbouiCOM.FormCreationParams;

				creationPackage.FormType = formType;
				creationPackage.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
				creationPackage.XmlData = formXml;

				if (formId != null)
					creationPackage.UniqueID = formId;

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