using System;

namespace sapHowmuch.Base.Extensions
{
	public static partial class ActionExtensions
	{
		public static void Forward<T>(this T value, Action<T> function) => function(value);

		public static void Forward<T1, T2>(this T1 value1, Action<T1, T2> function, T2 value2) => function(value1, value2);

		public static void Forward<T1, T2, T3>(this T1 value1, Action<T1, T2, T3> function, T2 value2, T3 value3) => function(value1, value2, value3);

		public static void Forward<T1, T2, T3, T4>(this T1 value1, Action<T1, T2, T3, T4> function, T2 value2, T3 value3, T4 value4) => function(value1, value2, value3, value4);
	}
}