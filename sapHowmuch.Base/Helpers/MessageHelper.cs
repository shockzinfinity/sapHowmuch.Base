using sapHowmuch.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sapHowmuch.Base.Helpers
{
	public static class MessageHelper
	{
		public static void SendMessage(string subject, string text, IEnumerable<string> userCodes, int? docNum = null, SAPbobsCOM.BoObjectTypes? boObjectTypes = null)
		{
			userCodes.ToList().ForEach(usercode => SendMessage(subject, text, usercode, docNum, boObjectTypes));
		}

		public static void SendMessage(string subject, string text, string userCode, int? docNum = null, SAPbobsCOM.BoObjectTypes? boObjectTypes = null)
		{
			var companyService = SapStream.DICompany.GetCompanyService();
			var messageService = companyService.GetBusinessService(SAPbobsCOM.ServiceTypes.MessagesService) as SAPbobsCOM.MessagesService;
			var message = messageService.GetDataInterface(SAPbobsCOM.MessagesServiceDataInterfaces.msdiMessage) as SAPbobsCOM.Message;

			try
			{
				message.Subject = subject;
				message.Text = text;
				message.RecipientCollection.Add();
				message.RecipientCollection.Item(0).SendInternal = SAPbobsCOM.BoYesNoEnum.tYES;
				message.RecipientCollection.Item(0).UserCode = userCode;

				// TODO: 각 오브젝트 별 메시지 데이터 컬럼 처리
				//if (docNum.HasValue && boObjectTypes.HasValue)
				//{
				//	var docEntry = docNum.Value.GetDocEntry("ORDR");
				//	if (docEntry.HasValue)
				//	{
				//		var column1 = message.MessageDataColumns.Add();
				//		column1.ColumnName = "Document";
				//		column1.Link = SAPbobsCOM.BoYesNoEnum.tYES;

				//		var line1 = column1.MessageDataLines.Add();
				//		line1.Value = docNum.ToString();
				//		line1.ObjectKey = docEntry.Value.ToString();
				//		line1.Object = ((int)boObjectTypes.Value).ToString();
				//	}
				//}

				messageService.SendMessage(message);
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"SendMessage Error: {ex.Message}");
				throw;
			}
			finally
			{
				message.ReleaseComObject();
				messageService.ReleaseComObject();
				companyService.ReleaseComObject();
			}
		}
	}
}