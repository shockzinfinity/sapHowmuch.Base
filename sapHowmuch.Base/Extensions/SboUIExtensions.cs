using sapHowmuch.Base.Helpers;
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
			foreach (SAPbouiCOM.MenuItem item in menus)
			{
				yield return item;
			}
		}

		public static IEnumerable<SAPbouiCOM.Item> AsEnumerable(this SAPbouiCOM.Items items)
		{
			foreach (SAPbouiCOM.Item item in items)
			{
				yield return item;
			}
		}

		public static bool Clear(this SAPbouiCOM.ValidValues values)
		{
			try
			{
				for (int i = values.Count - 1; i >= 1; i--)
				{
					values.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool Query(this SAPbouiCOM.ValidValues values, string query, bool addWhole = false, string wholeValue = "")
		{
			try
			{
				if (addWhole)
					values.Add(wholeValue, sapHowmuchConstants.ComboboxWholeDescription);

				using (var comboQuery = new SboRecordsetQuery(query))
				{
					if (comboQuery.Count == 0)
						return false;

					// just use first, second column
					foreach (var item in comboQuery.Result)
					{
						values.Add(item.Item(0).Value.ToString(), item.Item(1).Value.ToString());
					}
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}