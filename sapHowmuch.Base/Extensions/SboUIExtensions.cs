using System.Collections.Generic;

namespace sapHowmuch.Base.Extensions
{
	public static class SboUIExtensions
	{
		/// <summary>
		/// Return IEnumerable for LINQ support
		/// </summary>
		/// <param name="dbDatasources"></param>
		/// <returns></returns>
		public static IEnumerable<SAPbouiCOM.DBDataSource> AsEnumerable(this SAPbouiCOM.DBDataSources dbDatasources)
		{
			foreach (SAPbouiCOM.DBDataSource item in dbDatasources)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Return IEnumerable for LINQ support
		/// </summary>
		/// <param name="userDatasources"></param>
		/// <returns></returns>
		public static IEnumerable<SAPbouiCOM.UserDataSource> AsEnumerable(this SAPbouiCOM.UserDataSources userDatasources)
		{
			foreach (SAPbouiCOM.UserDataSource item in userDatasources)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Return IEnumerable for LINQ support
		/// </summary>
		/// <param name="dataTables"></param>
		/// <returns></returns>
		public static IEnumerable<SAPbouiCOM.DataTable> AsEnumerable(this SAPbouiCOM.DataTables dataTables)
		{
			foreach (SAPbouiCOM.DataTable item in dataTables)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Return IEnumerable for LINQ support
		/// </summary>
		/// <param name="menus"></param>
		/// <returns></returns>
		public static IEnumerable<SAPbouiCOM.MenuItem> AsEnumerable(this SAPbouiCOM.Menus menus)
		{
			foreach (SAPbouiCOM.MenuItem item in SapStream.UiApp.Menus)
			{
				yield return item;
			}
		}
	}
}