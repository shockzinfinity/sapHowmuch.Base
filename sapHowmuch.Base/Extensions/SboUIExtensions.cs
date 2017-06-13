using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			foreach (SAPbouiCOM.MenuItem item in SapStream.UiApp.Menus)
			{
				yield return item;
			}
		}
	}
}
