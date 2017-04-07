using System.Collections.Generic;
using System.Linq;
using System.Management;

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
	}
}