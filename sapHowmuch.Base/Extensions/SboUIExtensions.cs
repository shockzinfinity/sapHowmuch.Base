using System.Collections.Generic;

namespace sapHowmuch.Base.Extensions
{
	public static class SboUIExtensions
	{
		//public static SAPbouiCOM.DataTable GetDataTable(this SAPbouiCOM.Form form)
		//{
		//	form.DataSources.DataTables.GetEnumerator
		//}

		public static IEnumerable<SAPbouiCOM.MenuItem> AsEnumerable(this SAPbouiCOM.Menus menus)
		{
			foreach (SAPbouiCOM.MenuItem item in menus)
			{
				yield return item;
			}
		}
	}
}