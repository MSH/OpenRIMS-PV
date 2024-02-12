using System;

namespace OpenRIMS.PV.Main.Core.Utilities
{
    public static class DateTimeExtensions
	{
		public static int AgeAt(this DateTime date, DateTime other)
		{
			return other.Year - date.Year + (date.DayOfYear < other.DayOfYear ? -1 : 0);
		}

		public static DateTime ToStartOfMonth(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1);
		}

		public static DateTime ToEndOfMonth(this DateTime date)
		{
			return date.ToStartOfMonth().AddMonths(1).AddDays(-1);
		}
	}
}
