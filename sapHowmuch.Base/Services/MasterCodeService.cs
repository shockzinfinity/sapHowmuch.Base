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
		private static MasterCodeService Instance => _instance ?? (_instance = new MasterCodeService());

		public MasterCodeService()
		{
			Init();
		}

		private bool Init()
		{
			if (_setupOk) return true;

			try
			{
				UserDefinedHelper.CreateTable("MC0001H", "Master code table (H)")
					.CreateUdf("Remark", "remark", size: 254);

				UserDefinedHelper.CreateTable("MC0001L", "Master code table (L)")
					.CreateUdf("Code", "master code", size: 100)
					.CreateUdf("Name", "master name", size: 254)
					.CreateUdf("Value01", "value 1")
					.CreateUdf("Value02", "value 2")
					.CreateUdf("Value03", "value 3")
					.CreateUdf("Value04", "value 4");

				_setupOk = true;

				sapHowmuchLogger.Info("MasterCodeService initialized [OK]");
			}
			catch (Exception)
			{
			}

			return _setupOk;
		}
	}
}
