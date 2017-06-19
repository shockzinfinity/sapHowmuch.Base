using sapHowmuch.Base.Enums;
using System;
using System.Reflection;
using System.Xml;

namespace sapHowmuch.Base.Installer
{
	public abstract class AddonInfo : IAddonInfo
	{
		#region IAddonInfo implementation

		public virtual string PartnerName => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;

		public virtual string PartnerNamespace => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;

		public virtual string PartnerContact => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

		public abstract string AddonName { get; }

		// '.' is ignored.
		public abstract int AddonVersion { get; }

		// disable mandatory feature, modify in SBO client
		public string AddonGroup { get { return AddonGroupType.NoMandatory; } }

		public virtual string ClientType { get { return AddonClientType.All; } }

		public virtual string Platform { get { return AddonPlatform.x86; } }

		public virtual int EstimatedInstallTime { get; }

		public virtual string InstallCommandArgs { get; }

		public string SlientInstall { get { return "N"; } }

		public virtual int EstimatedUnInstallTime { get; }

		public virtual string UnInstallCommandArgs { get { return "/u"; } }

		public string SlientUninstall { get { return "N"; } }

		public abstract string InstallPath { get; set; }

		public abstract string DllPath { get; set; }

		#endregion IAddonInfo implementation

		protected virtual XmlDocument ToXml()
		{
			throw new NotImplementedException();
		}

		protected virtual string ToJson()
		{
			throw new NotImplementedException();
		}
	}

	public interface IAddonInfo
	{
		// partner info
		string PartnerName { get; }

		string PartnerNamespace { get; }
		string PartnerContact { get; }

		// addon info
		string AddonName { get; }

		int AddonVersion { get; }
		string AddonGroup { get; }
		string ClientType { get; }
		string Platform { get; }

		// install info
		int EstimatedInstallTime { get; }

		string InstallCommandArgs { get; }
		string SlientInstall { get; } // will not use

		// uninstall info
		int EstimatedUnInstallTime { get; }

		string UnInstallCommandArgs { get; }
		string SlientUninstall { get; } // will not use

		// will not use upgrade feature

		// parameter from SBO client
		string InstallPath { get; }
		string DllPath { get; }
	}
}