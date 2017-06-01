using sapHowmuch.Base.Helpers;
using System.Linq;
using System.Text.RegularExpressions;

namespace sapHowmuch.Base.Extensions
{
	/// <summary>
	/// SAP Business One User Query
	/// </summary>
	public static class UserQueryExtensions
	{
		public static string GetOrCreateUserQuery(this SAPbobsCOM.Company company, string userQueryName, string userQueryDefaultQuery, ParameterFormat parameterFormat = ParameterFormat.Sbo)
		{
			var userQuery = userQueryDefaultQuery;

			using (var userQueryObject = new SboRecordsetQuery<SAPbobsCOM.UserQueries>($"SELECT [IntrnalKey] FROM [OUQR] WHERE [QName] = '{userQueryName}'", SAPbobsCOM.BoObjectTypes.oUserQueries))
			{
				if (userQueryObject.Count == 0)
				{
					userQueryObject.BusinessObject.QueryDescription = userQueryName;
					userQueryObject.BusinessObject.Query = userQueryDefaultQuery;
					userQueryObject.BusinessObject.QueryCategory = -1;
					var response = userQueryObject.BusinessObject.Add();

					ErrorHelper.HandleErrorWithException(response, $"Could not create user query '{userQueryName}'");
				}
				else
				{
					userQuery = userQueryObject.Result.First().Query;
				}
			}

			switch (parameterFormat)
			{
				case ParameterFormat.Sql: userQuery = Regex.Replace(userQuery, @"'?\[%([0-9])\]'?", "@p$1"); break;
				case ParameterFormat.String: userQuery = Regex.Replace(userQuery, @"'?\[%([0-9])\]'?", "{$1}"); break;
			}

			return userQuery;
		}
	}

	public enum ParameterFormat
	{
		/// <summary>
		/// SAP Business One style [%0]
		/// </summary>
		Sbo,

		/// <summary>
		/// Sql style @p0
		/// </summary>
		Sql,

		/// <summary>
		/// string style {0}
		/// </summary>
		String
	}
}