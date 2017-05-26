using SAPbobsCOM;
using sapHowmuch.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace sapHowmuch.Base.Extensions
{
	public static class RecordsetExtensions
	{
		public static List<T> FillDataByRecordsetFieldAttribute<T>(this Recordset recordset, string query) where T : class
		{
			List<T> retList = new List<T>();

			try
			{
				if (string.IsNullOrWhiteSpace(query))
					throw new ArgumentNullException(nameof(query));

				recordset.DoQuery(query);

				if (recordset.RecordCount > 0)
				{
					while (!recordset.EoF)
					{
						T instance = Activator.CreateInstance(typeof(T), null) as T;
						var properties = typeof(T).GetPropertiesBySpecific<RecordsetFieldAttribute>();

						// TODO: subtype 에 따른 환경설정 소수점 자리수 적용 필요
						// db_Alpha(0), db_Memo(1) -> string
						// db_Numeric(2) -> int
						// db_Date(3) -> datetime
						// db_Float(4) -> decimal

						foreach (PropertyInfo item in properties)
						{
							item.SetValue(instance, Convert.ChangeType(recordset.Fields.Item(item.GetAttributeValueBy<RecordsetFieldAttribute>(f => f.FieldName)).Value, item.PropertyType), null);
						}

						retList.Add(instance);

						recordset.MoveNext();
					}
				}

				return retList;
			}
			catch
			{
				throw;
			}
			finally
			{
			}
		}
	}
}