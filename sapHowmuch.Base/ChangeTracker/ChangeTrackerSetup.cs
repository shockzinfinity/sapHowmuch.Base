using sapHowmuch.Base.Helpers;
using sapHowmuch.Base.Setup;

namespace sapHowmuch.Base.ChangeTracker
{
	public class ChangeTrackerSetup : ISetup
	{
		public int Version => 1;

		public void Run()
		{
			UserDefinedHelper.CreateTable(sapHowmuchConstants.CT_UdtName, "Change Tracker")
				.CreateUdf(sapHowmuchConstants.CT_UdfKey, "Key")
				.CreateUdf(sapHowmuchConstants.CT_UdfObj, "Object Type");
		}
	}
}