using System;
using System.Globalization;

namespace sapHowmuch.Base.Extensions
{
	public static class DataTypeExtensions
	{
		public static DateTime? ToDate(this SAPbouiCOM.UserDataSource datasource)
		{
			if (string.IsNullOrWhiteSpace(datasource.ValueEx))
				return null;

			return DateTime.ParseExact(datasource.ValueEx, "yyyyMMdd", CultureInfo.InvariantCulture);
		}

		// TODO: 다른 데이터 타입 추가 필요
	}
}