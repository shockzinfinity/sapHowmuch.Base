using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Dialogs;
using sapHowmuch.Base.Dialogs.Inputs;
using sapHowmuch.Base.Helpers;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace sapHowmuch.Base.Services
{
	public class SettingService : ISettingService
	{
		private bool _setupOk;
		private static SettingService _instance;
		public static SettingService Instance => _instance ?? (_instance = new SettingService());

		public SettingService()
		{
			Init();
		}

		private bool Init()
		{
			if (_setupOk) return true;

			try
			{
				UserDefinedHelper.CreateTable(sapHowmuchConstants.SettingTableName, sapHowmuchConstants.SettingTableDescription)
					.CreateUdf(sapHowmuchConstants.SettingFieldName, sapHowmuchConstants.SettingFieldDescription, size: 254);

				_setupOk = true;

				sapHowmuchLogger.Info("SettingService init [OK]");
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"SettingService Init [NOT OK] {ex.Message}");
				_setupOk = false;
				throw;
			}

			return _setupOk;
		}

		private string GetSettingAsString(string key, string userCode = null)
		{
			var sqlKey = key.Trim().ToLowerInvariant();

			if (!string.IsNullOrWhiteSpace(userCode))
				sqlKey = $"{sqlKey}[{userCode}]";

			var sql = $"SELECT [U_{sapHowmuchConstants.SettingFieldName}], [Name] FROM [@{sapHowmuchConstants.SettingTableName}] WHERE [Code] = '{sqlKey}'";

			using (var query = new SboRecordsetQuery(sql))
			{
				if (query.Count == 0) return null;

				var result = query.Result.First();
				return result.Item(0).Value as string;
			}
		}

		private static string GetSettingTitle(string key)
		{
			var sqlKey = key.Trim().ToLowerInvariant();
			var sql = $"SELECT [Name] FROM [@{sapHowmuchConstants.SettingTableName}] WHERE [Code] = '{sqlKey}'";

			using (var query = new SboRecordsetQuery(sql))
			{
				if (query.Count == 0) return key;

				var result = query.Result.First();
				var name = result.Item(0).Value as string;

				return string.IsNullOrWhiteSpace(name) ? key : name;
			}
		}

		private static TypeConverter GetTypeConverter(Type type)
		{
			return TypeDescriptor.GetConverter(type);
		}

		/// <summary>
		/// Converts a value to a destination type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		private static T To<T>(object value)
		{
			return (T)To(value, typeof(T));
		}

		private static object To(object value, Type destinationType)
		{
			return To(value, destinationType, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts a value to a destination type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		private static object To(object value, Type destinationType, CultureInfo culture)
		{
			if (value == null)
				return destinationType.IsValueType ? Activator.CreateInstance(destinationType) : null;

			var sourceType = value.GetType();
			var destinationConverter = GetTypeConverter(destinationType);
			var sourceConverter = GetTypeConverter(sourceType);

			if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
				return destinationConverter.ConvertFrom(null, culture, value);

			if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
				return sourceConverter.ConvertTo(null, culture, value, destinationType);

			if (destinationType.IsEnum && value is int)
				return Enum.ToObject(destinationType, (int)value);

			if (!destinationType.IsInstanceOfType(value))
				return Convert.ChangeType(value, destinationType, culture);

			return value;
		}

		public T GetCurrentUserSettingByKey<T>(string key, T defaultValue = default(T), bool askIfNotFound = false)
		{
			var usercode = SapStream.DICompany.UserName;

			return GetSettingByKey(key, defaultValue, usercode, askIfNotFound);
		}

		public T GetSettingByKey<T>(string key, T defaultValue = default(T), string userCode = null, bool askIfNotFound = false)
		{
			Init();

			if (string.IsNullOrEmpty(key))
				return defaultValue;

			var returnValue = defaultValue;
			var notFound = true;

			try
			{
				var value = GetSettingAsString(key, userCode);

				if (!string.IsNullOrWhiteSpace(value))
					notFound = false;

				if (string.IsNullOrWhiteSpace(value))
					value = null;

				returnValue = To<T>(value);
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"SettingService Error: {ex.Message} (Key = {key}, UserCode = {userCode})");
				return returnValue;
			}

			if (!notFound || !askIfNotFound)
				return returnValue;

			var name = GetSettingTitle(key);
			var inputTitle = $"Insert setting {name}";
			if (string.IsNullOrWhiteSpace(userCode))
				inputTitle += $" for {userCode}";

			var input = new TextDialogInput("setting", name, required: true) as IDialogInput;

			if (typeof(T) == typeof(bool))
				input = new CheckboxDialogInput("setting", name);

			if (typeof(T) == typeof(DateTime))
				input = new DateDialogInput("setting", name, required: true);

			if (typeof(T) == typeof(int))
				input = new IntegerDialogInput("setting", name, required: true);

			if (typeof(T) == typeof(decimal))
				input = new DecimalDialogInput("setting", name, required: true);

			var result = InputHelper.GetInputs(inputTitle).AddInput(input).Result();

			var newSetting = result.First().Value;

			SaveSetting(key, newSetting, userCode);

			returnValue = To<T>(newSetting);

			return returnValue;
		}

		public void InitSetting<T>(string key, string name, T defaultValue = default(T))
		{
			if (GetSettingAsString(key) == null)
				SaveSetting(key, defaultValue, name: name);
		}

		public void SaveCurrentUserSetting<T>(string key, T value)
		{
			var userCode = SapStream.DICompany.UserName;

			SaveSetting(key, value, userCode);
		}

		public void SaveSetting<T>(string key, T value = default(T), string userCode = null, string name = null)
		{
			Init();

			var sqlKey = key.Trim().ToLowerInvariant();

			if (userCode != null)
				sqlKey = $"{key}[{userCode}]";

			if (sqlKey.Length > 30)
				throw new Exception($"Sql Key '{sqlKey}' for setting is too long (max 30, actual {sqlKey.Length})");

			var sql = $"SELECT [U_{sapHowmuchConstants.SettingFieldName}], [Name] FROM [@{sapHowmuchConstants.SettingTableName}] WHERE [Code] = '{sqlKey}'";
			bool exists;

			using (var query = new SboRecordsetQuery(sql))
			{
				exists = query.Count == 1;
			}

			var sqlValue = string.Format(CultureInfo.InvariantCulture, "'{0}'", value);

			if (value == null)
				sqlValue = "NULL";

			if (exists)
			{
				sql = $"UPDATE [@{sapHowmuchConstants.SettingTableName}] SET [U_{sapHowmuchConstants.SettingFieldName}] = {sqlValue} WHERE [Code] = '{sqlKey}'";
			}
			else
			{
				if (sqlValue.Length > 254)
					throw new Exception($"SaveSetting sqlValue '{sqlValue}' value is too long (max 254)");

				if (string.IsNullOrWhiteSpace(name))
					name = sqlKey;

				if (name.Length > 30)
					name = name.Substring(0, 30); // max length 30

				sql = $"INSERT INTO [@{sapHowmuchConstants.SettingTableName}] ([Code], [Name], [U_{sapHowmuchConstants.SettingFieldName}]) VALUES ('{sqlKey}', '{name}', {sqlValue})";
			}

			SboRecordset.NonQuery(sql);
		}
	}
}