using sapHowmuch.Base.Installer;
using sapHowmuch.Base.TestWinform;
using System.Reflection;

namespace sapHowmuch.Base.TestWinformInstaller
{
	public class TestAddonInfo : AddonInfo
	{
		#region AddonInfo implementation

		public override string AddonName => Assembly.GetAssembly(typeof(TestWinformContext)).GetCustomAttribute<AssemblyTitleAttribute>().Title;

		public override string AddonVersion => Assembly.GetAssembly(typeof(TestWinformContext)).GetCustomAttribute<AssemblyVersionAttribute>().Version;

		public override string InstallPath { get; set; }

		public override string DllPath { get; set; }

		#endregion AddonInfo implementation
	}
}