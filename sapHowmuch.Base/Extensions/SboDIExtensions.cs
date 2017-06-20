using System.Collections.Generic;

namespace sapHowmuch.Base.Extensions
{
	public static class SboDIExtensions
	{
		public static IEnumerable<SAPbobsCOM.Field> AsEnumerable(this SAPbobsCOM.Fields fields)
		{
			foreach (var item in fields)
			{
				yield return (SAPbobsCOM.Field)item;
			}
		}

		public static SAPbobsCOM.Recordset GetRecordset(this SAPbobsCOM.Company company)
		{
			return company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
		}
	}
}