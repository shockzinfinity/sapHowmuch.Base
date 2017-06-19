using sapHowmuch.Base.Installer;
using sapHowmuch.Base.TestWinform;
using System;
using System.Reflection;

namespace sapHowmuch.Base.TestWinformInstaller
{
	public class TestAddonInfo : AddonInfo
	{
		#region AddonInfo implementation

		public override string AddonName => Assembly.GetAssembly(typeof(TestWinformContext)).GetCustomAttribute<AssemblyTitleAttribute>().Title;

		public override int AddonVersion => Convert.ToInt32(Assembly.GetAssembly(typeof(TestWinformContext)).GetCustomAttribute<AssemblyFileVersionAttribute>().Version.Replace(".", ""));

		public override string InstallPath { get; set; }

		public override string DllPath { get; set; }

		#endregion AddonInfo implementation
	}
}