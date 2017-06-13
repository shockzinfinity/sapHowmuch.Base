using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Helpers;
using sapHowmuch.Base.Services;
using sapHowmuch.Base.Setup;

namespace sapHowmuch.Base.TestWinform
{
	public class TestSetup : ISetup
	{
		public int Version => 2; // TODO: assembly version 정보로 대체

		public void Run()
		{
			// for test
			UserDefinedHelper.CreateTable("SHM_TestTbl1", "test table")
				.CreateUdf("test_col1", "test column 1", SAPbobsCOM.BoFieldTypes.db_Alpha, 30)
				.CreateUdf("test_col2", "test column 2", SAPbobsCOM.BoFieldTypes.db_Numeric, 10)
				.CreateUdf("test_col3", "test column 3", SAPbobsCOM.BoFieldTypes.db_Numeric, 10);

			UserDefinedHelper.CreateField(SboTable.BusinessPartner, "SHM_YesNo", "Yes or no?", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, validValues: UserDefinedHelper.YesNoValidValues);

			SettingService.Instance.InitSetting("sapHowmuch.savePath", "Save path", @"C:\temp\");
		}
	}
}