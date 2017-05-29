using System;
using System.Xml;

namespace sapHowmuch.Base.Extensions
{
	public static class BusinessObjectInfoExtensions
	{
		public static int GetDocEntry(this SAPbouiCOM.BusinessObjectInfo businessObjectInfo)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(businessObjectInfo.ObjectKey);

			return int.Parse(xmlDoc.SelectSingleNode("/DocumentParams/DocEntry").InnerText);
		}

		public static bool GetByDocNum<T>(this BusinessObject<T> businessObject, int docNum) where T : SAPbobsCOM.Documents
		{
			return businessObject.Object.Search(businessObject.BoObjectType.GetTableName(), $"[DocNum] = {docNum}");
		}

		public static BusinessObject<T> GetBusinessObject<T>(this SAPbobsCOM.Company company, SAPbobsCOM.BoObjectTypes boObjectTypes)
		{
			return new BusinessObject<T>(company, boObjectTypes);
		}
	}

	public class BusinessObject<T> : IDisposable
	{
		public readonly SAPbobsCOM.BoObjectTypes BoObjectType;
		public readonly T Object;

		public BusinessObject(SAPbobsCOM.Company company, SAPbobsCOM.BoObjectTypes boObjectType)
		{
			this.BoObjectType = boObjectType;
			this.Object = (T)company.GetBusinessObject(BoObjectType);
		}

		public void Dispose()
		{
			if (Object != null) Object.ReleaseComObject();
		}
	}
}