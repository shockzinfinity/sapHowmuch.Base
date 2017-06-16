using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;

namespace sapHowmuch.Base
{
	public static partial class ProcessHelper
	{
		public static IEnumerable<Win32Process> All(ManagementScope managementScope = null) =>
			Wmi
			.Query($"SELECT * FROM {Win32Process.WmiClassName}", managementScope)
			.Select(process => new Win32Process(process));

		public static Win32Process ById(uint processId, ManagementScope managementScope = null) =>
			Wmi
			.Query($"SELECT * FROM {Win32Process.WmiClassName} WHERE {nameof(Win32Process.ProcessId)} = {processId}", managementScope)
			.Select(process => new Win32Process(process))
			.FirstOrDefault();

		public static IEnumerable<Win32Process> ByName(string name, ManagementScope managementScope = null) =>
			Wmi
			.Query($"SELECT * FROM {Win32Process.WmiClassName} WHERE {nameof(Win32Process.Name)} = '{name}'", managementScope)
			.Select(process => new Win32Process(process));

		public static IEnumerable<Win32Process> ChildProcesses(uint parentProcessId, ManagementScope managementScope = null) =>
			Wmi
			.Query($"SELECT * FROM {Win32Process.WmiClassName} WHERE {nameof(Win32Process.ParentProcessId)} = {parentProcessId}", managementScope)
			.Select(process => new Win32Process(process));

		public static IEnumerable<Win32Process> AllChildProcesses(uint parentProcessId, ManagementScope managementScope = null)
		{
			IEnumerable<Win32Process> childProcesses =
				Wmi
				.Query($"SELECT * FROM {Win32Process.WmiClassName} WHERE {nameof(Win32Process.ParentProcessId)} = {parentProcessId}", managementScope)
				.Select(process => new Win32Process(process));

			return childProcesses.Concat(childProcesses.SelectMany(process => process.ProcessId.HasValue ? AllChildProcesses(process.ProcessId.Value, managementScope) : Enumerable.Empty<Win32Process>()));
		}

		/// <summary>
		/// Run as Administrator's permission
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static bool Execute(string fileName, string[] args)
		{
			bool retVal = true;

			// vista 미만일때는 관리자 권한으로 실행할 필요가 없음.
			if (Environment.OSVersion.Version.Major < 6) return true;

			if (!File.Exists(fileName))
				throw new ApplicationException($"Could not find file: {fileName}");

			FileInfo fi = new FileInfo(fileName);

			ProcessStartInfo proc = new ProcessStartInfo();
			proc.UseShellExecute = true;
			proc.WorkingDirectory = fi.DirectoryName;
			proc.FileName = fi.Name;

			string argument = string.Empty;

			if (args != null)
			{
				for (int i = 0; i < args.Length; i++)
				{
					if (string.IsNullOrWhiteSpace(argument))
					{
						argument = args[i];
					}
					else
					{
						argument += " " + args[i];
					}
				}
			}

			proc.Arguments = argument;
			proc.Verb = "runas";

			Process.Start(proc);

			return retVal;
		}

		public static bool IsRunAsAdministrator()
		{
			bool isAdmin = false;

			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new WindowsPrincipal(identity);
			isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

			return isAdmin;
		}
	}
}