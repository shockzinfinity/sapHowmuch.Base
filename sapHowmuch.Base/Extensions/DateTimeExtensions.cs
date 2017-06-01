using sapHowmuch.Base.Enums;
using System;
using System.Globalization;

namespace sapHowmuch.Base.Extensions
{
	public static class DateTimeExtensions
	{
		private static readonly DateTime _unixReferenceDateTime = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).ToUniversalTime();

		public static DateTime GetLastDayInMonth(this DateTime source)
		{
			DateTime result = DateTime.ParseExact(source.ToString("yyyyMM01"), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

			return result.AddMonths(1).AddDays(-1);
		}

		public static int GetQuarter(this DateTime source)
		{
			DateTime result = source;
			return (result.Month - 1) / 3 + 1;
		}

		public static DateTime[] GetHalfPeriod(this DateTime source, bool getSecondHalf = true)
		{
			int year = source.Year;
			int startMonth = getSecondHalf ? 1 : 7;
			int endMonth = getSecondHalf ? 6 : 12;

			DateTime[] dates = new DateTime[2];

			dates[0] = new DateTime(year, startMonth, 1);
			dates[1] = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));

			return dates;
		}

		public static DateTime[] GetQuarterPeriodBy(this DateTime source, int quarter)
		{
			if (quarter > 4) quarter = 1;

			DateTime startDate = new DateTime(source.Year, (quarter * 3) - 2, 1);

			return new DateTime[2]{
				startDate,
				new DateTime(source.Year, startDate.AddMonths(2).Month, DateTime.DaysInMonth(source.Year, startDate.AddMonths(2).Month))
			};
		}

		public static DateTime[] GetCurrentQuarterPeriod(this DateTime source)
		{
			return source.GetQuarterPeriodBy(source.GetQuarter());
		}

		public static bool IsSecondHalf(this DateTime source)
		{
			return source.Month < 6;
		}

		public static bool IsBetween(this DateTime source, DateTime lower, DateTime upper, bool ignoreTime = true)
		{
			if (ignoreTime)
			{
				source = source.Date;
				lower = lower.Date;
				upper = upper.Date;
			}

			return source.CompareTo(lower) >= 0 && source.CompareTo(upper) <= 0;
		}

		public static double DateDiff(this DateTime baseDate, DateTime compareDate, DateTimeInterval interval)
		{
			double diff = 0;
			TimeSpan ts = baseDate - compareDate;

			switch (interval)
			{
				case DateTimeInterval.Year:
					ts = DateTime.Parse(compareDate.ToString("yyyy-01-01")) - DateTime.Parse(baseDate.ToString("yyyy-01-01"));
					diff = Convert.ToDouble(ts.TotalDays / 365);
					break;

				case DateTimeInterval.Month:
					ts = DateTime.Parse(compareDate.ToString("yyyy-MM-01")) - DateTime.Parse(baseDate.ToString("yyyy-MM-01"));
					diff = Convert.ToDouble((ts.TotalDays / 365) * 12);
					break;

				case DateTimeInterval.Day:
					ts = DateTime.Parse(compareDate.ToString("yyyy-MM-dd")) - DateTime.Parse(baseDate.ToString("yyyy-MM-dd"));
					diff = ts.Days;
					break;

				case DateTimeInterval.Hour:
					ts = DateTime.Parse(compareDate.ToString("yyyy-MM-dd HH:00:00")) - DateTime.Parse(baseDate.ToString("yyyy-MM-dd HH:00:00"));
					diff = ts.TotalHours;
					break;

				case DateTimeInterval.Minute:
					ts = DateTime.Parse(compareDate.ToString("yyyy-MM-dd HH:mm:00")) - DateTime.Parse(baseDate.ToString("yyyy-MM-dd HH:mm:00"));
					diff = ts.TotalMinutes;
					break;

				case DateTimeInterval.Second:
					ts = DateTime.Parse(compareDate.ToString("yyyy-MM-dd HH:mm:ss")) - DateTime.Parse(baseDate.ToString("yyyy-MM-dd HH:mm:ss"));
					diff = ts.TotalSeconds;
					break;

				case DateTimeInterval.MiliSecond:
					diff = ts.TotalMilliseconds;
					break;
			}

			return diff;
		}

		/// <summary>
		/// 1970년 1월 1일 부터 얼마나 시간이 지났는지
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static long GetUnixTime(this DateTime dateTime)
		{
			return (long)(dateTime.ToUniversalTime() - _unixReferenceDateTime).TotalSeconds;
		}

		/// <summary>
		/// 특정날짜(혹은 현재날짜)로 부터 얼마나 경과했는지
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static string When(this DateTime dateTime)
		{
			return dateTime.When(DateTime.Now);
		}

		/// <summary>
		/// 해당 날짜로부터 경과한 시간
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="currentTime"></param>
		/// <returns></returns>
		public static string When(this DateTime dateTime, DateTime currentTime)
		{
			var timespan = currentTime - dateTime;

			if (timespan.Days > 365) return string.Format("{0} year{1} ago", timespan.Days / 365, (timespan.Days / 365) > 1 ? "s" : "");
			if (timespan.Days > 30) return string.Format("{0} month{1} ago", timespan.Days / 30, (timespan.Days / 30) > 1 ? "s" : "");
			if (timespan.Days > 0) return string.Format("{0} day{1} ago", timespan.Days, timespan.Days > 1 ? "s" : "");
			if (timespan.Hours > 0) return string.Format("{0} hour{1} ago", timespan.Hours, timespan.Hours > 1 ? "s" : "");
			if (timespan.Minutes > 0) return string.Format("{0} minute{1} ago", timespan.Minutes, timespan.Minutes > 1 ? "s" : "");

			return "A moment ago";
		}

		/// <summary>
		/// 두 시작/종료일자가 교차하는지 여부
		/// <para>
		/// bool eventsInterect = eventXStartDate.Intersects(eventXEndDate, eventYStartDate, eventYEndDate);
		/// </para>
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="intersectingStartDate"></param>
		/// <param name="intersectingEnddate"></param>
		/// <returns></returns>
		public static bool Intersects(this DateTime startDate, DateTime endDate, DateTime intersectingStartDate, DateTime intersectingEnddate)
		{
			return (intersectingEnddate >= startDate && intersectingStartDate <= endDate);
		}
	}
}