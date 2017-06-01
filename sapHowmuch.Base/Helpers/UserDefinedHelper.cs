using sapHowmuch.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sapHowmuch.Base.Helpers
{
	public static class UserDefinedHelper
	{
		public static Dictionary<string, string> YesNoValidValues => new Dictionary<string, string>
		{
			{ "Y", "Yes" }, { "N", "No" }
		};

		public class UserDefinedTable
		{
			public string TableName { get; set; }

			public UserDefinedTable(string tableName)
			{
				TableName = tableName;
			}

			public UserDefinedTable CreateUdf(string fieldName, string fieldDescription, SAPbobsCOM.BoFieldTypes type = SAPbobsCOM.BoFieldTypes.db_Alpha, int size = 50, SAPbobsCOM.BoFldSubTypes subType = SAPbobsCOM.BoFldSubTypes.st_None, IDictionary<string, string> validValues = null, string defaultValue = null)
			{
				CreateField(TableName, fieldName, fieldDescription, type, size, subType, validValues, defaultValue);
				return this;
			}
		}

		public static UserDefinedTable CreateTable(string tableName, string tableDescription, SAPbobsCOM.BoUTBTableType tableType = SAPbobsCOM.BoUTBTableType.bott_NoObject)
		{
			SAPbobsCOM.UserTablesMD userTablesMd = null;

			try
			{
				userTablesMd = SapStream.DICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables) as SAPbobsCOM.UserTablesMD;

				if (userTablesMd == null)
					throw new NullReferenceException("Failed to get UserTablesMD object");

				if (!userTablesMd.GetByKey(tableName))
				{
					userTablesMd.TableName = tableName;
					userTablesMd.TableDescription = tableDescription;
					userTablesMd.TableType = tableType;

					ErrorHelper.HandleErrorWithException(userTablesMd.Add(), $"Could not create UDT {tableName}");
				}
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"UDT Create error: {ex.Message}");
				throw;
			}
			finally
			{
				if (userTablesMd != null) userTablesMd.ReleaseComObject();
			}

			return new UserDefinedTable("@" + tableName);
		}

		public static void CreateFieldOnUDT(string tableName, string fieldName, string fieldDescription, SAPbobsCOM.BoFieldTypes type = SAPbobsCOM.BoFieldTypes.db_Alpha, int size = 50, SAPbobsCOM.BoFldSubTypes subType = SAPbobsCOM.BoFldSubTypes.st_None)
		{
			tableName = "@" + tableName;
			CreateField(tableName, fieldName, fieldDescription, type, size, subType);
		}

		public static void CreateField(string tableName, string fieldName, string fieldDescription, SAPbobsCOM.BoFieldTypes type = SAPbobsCOM.BoFieldTypes.db_Alpha, int size = 50, SAPbobsCOM.BoFldSubTypes subType = SAPbobsCOM.BoFldSubTypes.st_None, IDictionary<string, string> validValues = null, string defaultValue = null)
		{
			SAPbobsCOM.UserFieldsMD userFieldMd = null;

			try
			{
				userFieldMd = SapStream.DICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields) as SAPbobsCOM.UserFieldsMD;

				if (userFieldMd == null)
					throw new NullReferenceException("Failed to get UserFieldMD object");

				var fieldId = GetFieldId(tableName, fieldName);
				if (fieldId != -1) return;

				userFieldMd.TableName = tableName;
				userFieldMd.Name = fieldName;
				userFieldMd.Description = fieldDescription;
				userFieldMd.Type = type;
				userFieldMd.SubType = subType;
				userFieldMd.Size = size;
				userFieldMd.EditSize = size;
				userFieldMd.DefaultValue = defaultValue;

				if (validValues != null)
				{
					foreach (var validValue in validValues)
					{
						userFieldMd.ValidValues.Value = validValue.Key;
						userFieldMd.ValidValues.Description = validValue.Value;
						userFieldMd.ValidValues.Add();
					}
				}

				ErrorHelper.HandleErrorWithException(userFieldMd.Add(), "Could not create field");
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"Create field {tableName}.{fieldName} error: {ex.Message}");
				throw;
			}
			finally
			{
				if (userFieldMd != null) userFieldMd.ReleaseComObject();
			}
		}

		public static int GetFieldId(string tableName, string fieldAlias)
		{
			using (var query = new SboRecordsetQuery($"SELECT [FieldID] FROM [CUFD] WHERE [TableID] = '{tableName}' AND [AliasID] = '{fieldAlias}'"))
			{
				if (query.Count == 1)
				{
					var fieldId = query.Result.First().Item(0).Value as int?;
					return fieldId.Value;
				}
			}

			return -1;
		}
	}
}