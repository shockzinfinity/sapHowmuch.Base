using sapHowmuch.Base.Helpers;
using System.Linq;

namespace sapHowmuch.Base.Extensions
{
	public static class BusinessPartnerExtensions
	{
		public static bool SetContactEmployeesLineByContactCode(this SAPbobsCOM.BusinessPartners businessPartners, int contactCode)
		{
			var cardCode = businessPartners.CardCode;
			var contactEmployees = businessPartners.ContactEmployees;

			using (var query = new SboRecordsetQuery($"SELECT [LineNum] FROM (SELECT ROW_NUMBER() OVER (ORDER BY [CntctCode] ASC) - 1 AS [LineNum], [CntctCode] FROM [OCPR] WHERE [CardCode] = '{cardCode}') AS [T0] WHERE [CntctCode] = {contactCode}"))
			{
				if (query.Count == 0) return false;

				var lineNum = (int)query.Result.First().Item(0).Value;
				sapHowmuchLogger.Debug($"CntctCode {contactCode} is LineNum {lineNum} for CardCode {cardCode}");

				contactEmployees.SetCurrentLine(lineNum);

				return true;
			}
		}

		public static bool SetContactEmployeesLineByContactId(this SAPbobsCOM.BusinessPartners businessPartners, string contactId)
		{
			var cardCode = businessPartners.CardCode;
			var contactEmployees = businessPartners.ContactEmployees;

			using (var query = new SboRecordsetQuery($"SELECT [LineNum] FROM (SELECT ROW_NUMBER() OVER (ORDER BY [CntctCode] ASC) - 1 AS [LineNum], [Name] FROM [OCPR] WHERE [CardCode] = '{cardCode}') AS [T0] WHERE [Name] = '{contactId}'"))
			{
				if (query.Count == 0) return false;

				var lineNum = (int)query.Result.First().Item(0).Value;
				sapHowmuchLogger.Debug($"Contact ID '{contactId}' is LineNum {lineNum} for CardCode {cardCode}");

				contactEmployees.SetCurrentLine(lineNum);

				return true;
			}
		}

		public static void SetNextCardCode(this SAPbobsCOM.BusinessPartners businessPartners, int? seriesId = null)
		{
			var response = DocumentSeriesHelper.GetNextNumber(SAPbobsCOM.BoObjectTypes.oBusinessPartners, "C", seriesId);
			businessPartners.CardCode = response.NextNumber;
			businessPartners.Series = response.Series;
		}
	}
}