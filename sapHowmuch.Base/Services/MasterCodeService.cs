using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sapHowmuch.Base.Services
{
	public class MasterCodeService
	{
		private bool _setupOk;
		private static MasterCodeService _instance;
		public static MasterCodeService Instance => _instance ?? (_instance = new MasterCodeService());

		public MasterCodeService()
		{
			Init();
		}

		private bool Init()
		{
			if (_setupOk) return true;

			try
			{
				UserDefinedHelper.CreateTable(sapHowmuchConstants.MasterHeaderTable, sapHowmuchConstants.MasterHeaderTableDescription, SAPbobsCOM.BoUTBTableType.bott_MasterData)
					.CreateUdf("Remark", "remark", size: 254);

				UserDefinedHelper.CreateTable(sapHowmuchConstants.MasterLineTable, sapHowmuchConstants.MasterLineTableDescription, SAPbobsCOM.BoUTBTableType.bott_MasterDataLines)
					.CreateUdf("IsUse", "Is use", SAPbobsCOM.BoFieldTypes.db_Alpha, size:1, validValues: UserDefinedHelper.YesNoValidValues)
					.CreateUdf(sapHowmuchConstants.MasterLineCodeField, "detail code", size: 50)
					.CreateUdf(sapHowmuchConstants.MasterLineNameField, "detail name", size: 150)
					.CreateUdf(sapHowmuchConstants.MasterLineValueField, "detail value", size: 254);

				_setupOk = true;

				sapHowmuchLogger.Info("MasterCodeService initialized [OK]");
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"MasterCodeService not initialize [NOT OK] {ex.Message}");
				_setupOk = false;
				throw;
			}

			return _setupOk;
		}

		public MasterCode GetBy(string code)
		{
			MasterCode retCode;

			var sql = $"SELECT H.[Code], H.[Name], H.[U_Remark], L.[U_{sapHowmuchConstants.MasterLineCodeField}], L.[U_{sapHowmuchConstants.MasterLineNameField}], L.[U_{sapHowmuchConstants.MasterLineValueField}] FROM [@{sapHowmuchConstants.MasterLineTable}] AS L INNER JOIN [@{sapHowmuchConstants.MasterHeaderTable}] AS H ON L.[Code] = H.[Code] WHERE H.[Code] = '{code}'";

			using (var query = new SboRecordsetQuery(sql))
			{
				if (query.Count == 0) return null;

				retCode = new MasterCode(
					query.Result.First().Item(0).Value.ToString().Trim(),
					query.Result.First().Item(1).Value.ToString().Trim(),
					query.Result.First().Item(2).Value.ToString().Trim(),
					details: query.Result.Select(x => new MasterCode(x.Item($"U_{sapHowmuchConstants.MasterLineCodeField}").Value.ToString().Trim(), x.Item($"U_{sapHowmuchConstants.MasterLineNameField}").Value.ToString().Trim(), x.Item($"U_{sapHowmuchConstants.MasterLineValueField}").Value.ToString().Trim(), true)).ToArray());
			}

			return retCode;
		}

		public void GetValidValues(SAPbouiCOM.ValidValues validValues, string code)
		{
			var result = GetBy(code);

			if (result.HasLine)
			{
				validValues.Clear();

				foreach (var item in result.Details)
				{
					validValues.Add(item.Code, item.Name);
				}
			}
		}
	}

	public class MasterCode
	{
		public string Code { get; private set; }
		public string Name { get; private set; }
		public string Value { get; private set; }
		public IEnumerable<MasterCode> Details { get; private set; }
		public bool HasLine { get { return Details != null && Details.Count() > 0; } }
		public bool IsDetail { get; private set; }

		public MasterCode(string code, string name, string value, bool isDetail = false, params MasterCode[] details)
		{
			this.Code = code;
			this.Name = name;
			this.Value = value;
			this.IsDetail = isDetail;
			this.Details = details;
		}
	}
}
