using System;
using System.Globalization;

namespace sapHowmuch.Base.Extensions
{
	public static class DataTypeExtensions
	{
		public static DateTime? ToDate(this SAPbouiCOM.UserDataSource datasource)
		{
			if (string.IsNullOrWhiteSpace(datasource.ValueEx.Trim()))
				return null;

			return DateTime.ParseExact(datasource.ValueEx.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
		}
	}
}