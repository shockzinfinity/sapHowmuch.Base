using System;
using System.Linq;
using System.Management;

namespace sapHowmuch.Base
{
	public static class Wmi
	{
		public static ManagementObject[] Query(ObjectQuery objectQuery, ManagementScope managementScope = null)
		{
			if (objectQuery == null)
			{
				throw new ArgumentNullException(nameof(objectQuery));
			}

			// managementScope  - Default : \\.\root\cimv2
			// objectQuery = WQL
			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(managementScope ?? new ManagementScope(), objectQuery))
			{
				using (ManagementObjectCollection processes = searcher.Get())
				{
					return processes.OfType<ManagementObject>().ToArray();
				}
			}
		}

		public static ManagementObject[] Query(string query, ManagementScope managementScope = null) => Query(new ObjectQuery(query), managementScope);
	}
}