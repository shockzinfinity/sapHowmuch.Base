using System;
using System.Runtime.InteropServices;

namespace sapHowmuch.Base.Installer
{
	/// <summary>
	/// SAP Installer API를 제공합니다.
	/// </summary>
	public class InstallAPIWrapper
	{
		public delegate int DInstApiParameterLess();

		public delegate int DInstApiString(string str);

		public delegate int DInstApiStringBool(string str, bool b);

		private static DInstApiParameterLess endInstall;
		private static DInstApiParameterLess restartNeeded;
		private static DInstApiParameterLess endUninstall;
		private static DInstApiString setAddOnFolder;

		/// <summary>
		/// API를 초기화 합니다.
		/// </summary>
		/// <param name="path"></param>
		public static void Init(string path)
		{
			SetDllDirectory(path);

			switch (IntPtr.Size * 8)
			{
				case 32:
					endInstall = EndInstall32;
					restartNeeded = RestartNeeded32;
					endUninstall = EndUninstall32;
					setAddOnFolder = SetAddOnFolder32;
					break;

				case 64:
					endInstall = EndInstall64;
					restartNeeded = RestartNeeded64;
					endUninstall = EndUninstall64;
					setAddOnFolder = SetAddOnFolder64;
					break;
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetDllDirectory(string lpPathName);

		[DllImport("AddOnInstallAPI.dll", EntryPoint = "EndInstall")]
		public static extern int EndInstall32();

		[DllImport("AddOnInstallAPI.dll", EntryPoint = "SetAddOnFolder")]
		public static extern int SetAddOnFolder32(string srrPath);

		[DllImport("AddOnInstallAPI.dll", EntryPoint = "RestartNeeded")]
		public static extern int RestartNeeded32();

		[DllImport("AddOnInstallAPI.dll", EntryPoint = "EndUninstall")]
		public static extern int EndUninstall32();

		[DllImport("AddOnInstallAPI_x64.dll", EntryPoint = "EndInstall")]
		public static extern int EndInstall64();

		[DllImport("AddOnInstallAPI_x64.dll", EntryPoint = "SetAddOnFolder")]
		public static extern int SetAddOnFolder64(string srrPath);

		[DllImport("AddOnInstallAPI_x64.dll", EntryPoint = "RestartNeeded")]
		public static extern int RestartNeeded64();

		[DllImport("AddOnInstallAPI_x64.dll", EntryPoint = "EndUninstall")]
		public static extern int EndUninstall64();

		public static int RestartNeeded()
		{
			return restartNeeded();
		}

		public static int SetAddOnFolder(string path)
		{
			return setAddOnFolder(path);
		}

		public static int EndInstall()
		{
			return endInstall();
		}

		public static int EndUninstall()
		{
			return endUninstall();
		}
	}
}