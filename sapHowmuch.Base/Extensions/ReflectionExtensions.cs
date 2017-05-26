using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace sapHowmuch.Base.Extensions
{
	public static class ReflectionExtensions
	{
		public static T GetDefaultValue<T>(this PropertyInfo propertyInfo)
		{
			if (propertyInfo.IsDefined(typeof(DefaultValueAttribute), false))
			{
				DefaultValueAttribute attribute = propertyInfo.GetCustomAttributes(false).Where(x => x.GetType() == typeof(DefaultValueAttribute)).FirstOrDefault() as DefaultValueAttribute;

				return (T)attribute.Value;
			}
			else
			{
				return default(T);
			}
		}

		public static T GetAttributeOfTypeBy<T>(this Enum enumValue) where T : Attribute
		{
			var type = enumValue.GetType();
			var memberInfo = type.GetMember(enumValue.ToString());
			var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

			return (attributes.Length > 0) ? (T)attributes[0] : null;
		}

		public static string GetAttributeValueBy<T>(this Enum enumValue, Func<T, string> expression) where T : Attribute
		{
			T attribute = enumValue.GetType().GetMember(enumValue.ToString())
				.Where(m => m.MemberType == MemberTypes.Field)
				.FirstOrDefault()
				.GetCustomAttributes(typeof(T), false)
				.Cast<T>()
				.SingleOrDefault();

			if (attribute == null)
				return default(string);

			return expression.Invoke(attribute);
		}

		public static string GetAttributeValueBy<T>(this PropertyInfo pi, Func<T, string> expression) where T : Attribute
		{
			T attribute = pi.GetCustomAttributes(typeof(T), false).Cast<T>().SingleOrDefault();

			return expression.Invoke(attribute);
		}

		public static bool GetAttributeValueBy<T>(this PropertyInfo pi, Func<T, bool> expression) where T : Attribute
		{
			T attribute = pi.GetCustomAttributes(typeof(T), false).Cast<T>().SingleOrDefault();

			return expression.Invoke(attribute);
		}

		public static TRequestedType InstantiateClass<TRequestedType>(this string className, params object[] constructorArgs)
		{
			var classType = Assembly.GetExecutingAssembly().GetExportedTypes().Where(x => x.Name == className).FirstOrDefault();

			return (TRequestedType)Activator.CreateInstance(classType, constructorArgs);
		}

		public static PropertyInfo[] GetPropertiesBySpecific<T>(this Type requestType) where T : Attribute
		{
			return requestType.GetProperties().Where(x => x.IsDefined(typeof(T), false)).ToArray();
		}
	}
}