using sapHowmuch.Base.Extensions;
using System;

namespace sapHowmuch.Base.Helpers
{
	public static class DocumentSeriesHelper
	{
		public static NextNumberResponse GetNextNumber(SAPbobsCOM.BoObjectTypes documentType, string docSubType = null, int? seriesId = null)
		{
			var companyService = SapStream.DICompany.GetCompanyService();
			var seriesService = companyService.GetBusinessService(SAPbobsCOM.ServiceTypes.SeriesService) as SAPbobsCOM.SeriesService;
			var docTypeParams = seriesService.GetDataInterface(SAPbobsCOM.SeriesServiceDataInterfaces.ssdiDocumentTypeParams) as SAPbobsCOM.DocumentTypeParams;

			try
			{
				docTypeParams.Document = ((int)documentType).ToString();

				if (docSubType != null)
				{
					docTypeParams.DocumentSubType = docSubType;
				}

				SAPbobsCOM.Series series = null;

				if (seriesId.HasValue)
				{
					var allSeries = seriesService.GetDocumentSeries(docTypeParams);

					foreach (SAPbobsCOM.Series s in allSeries)
					{
						if (s.Series == seriesId.Value)
						{
							series = s;
							break;
						}
					}

					if (series == null)
						throw new Exception($"Could not find series {seriesId.Value}");
				}
				else
				{
					series = seriesService.GetDefaultSeries(docTypeParams);
				}

				return new NextNumberResponse
				{
					NextNumber = $"{series.Prefix}{series.NextNumber}{series.Suffix}",
					Series = series.Series
				};
			}
			finally
			{
				seriesService.ReleaseComObject();
			}
		}
	}

	public class NextNumberResponse
	{
		public string NextNumber { get; set; }
		public int Series { get; set; }
	}
}