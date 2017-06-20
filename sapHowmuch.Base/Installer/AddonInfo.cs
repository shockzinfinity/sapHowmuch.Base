using sapHowmuch.Base.Enums;
using System;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace sapHowmuch.Base.Installer
{
	[XmlRoot("AddOnInfo")]
	public abstract class AddonInfo : IAddonInfo, IXmlSerializable
	{
		#region IAddonInfo implementation

		public virtual string PartnerName => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;

		public virtual string PartnerNamespace => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;

		public virtual string PartnerContact => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

		public abstract string AddonName { get; }

		public abstract string AddonVersion { get; }

		// disable mandatory feature, modify in SBO client
		public string AddonGroup { get { return AddonGroupType.NoMandatory; } }

		public virtual string ClientType { get { return AddonClientType.All; } }

		public virtual string Platform { get { return AddonPlatform.x86; } }

		public virtual int EstimatedInstallTime { get; }

		public virtual string InstallCommandArgs { get; }

		public string SilentInstall { get { return "N"; } }

		public virtual int EstimatedUnInstallTime { get; }

		public virtual string UnInstallCommandArgs { get { return "/u"; } }

		public string SilentUninstall { get { return "N"; } }

		public string EstimatedUpgradeTime { get; }
		public string UpgradeCommandArgs { get; }
		public string SilentUpgrade { get { return "N"; } }

		public abstract string InstallPath { get; set; }

		public abstract string DllPath { get; set; }

		#endregion IAddonInfo implementation

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("partnername", PartnerName);
			writer.WriteAttributeString("partnernmsp", PartnerNamespace);
			writer.WriteAttributeString("contdata", PartnerContact);
			writer.WriteAttributeString("addonname", AddonName);
			writer.WriteAttributeString("addongroup", AddonGroup);
			writer.WriteAttributeString("clienttype", ClientType);
			writer.WriteAttributeString("platform", Platform);
			writer.WriteAttributeString("esttime", EstimatedInstallTime.ToString());
			writer.WriteAttributeString("instparams", InstallCommandArgs);
			writer.WriteAttributeString("silentinst", SilentInstall);
			writer.WriteAttributeString("unesttime", EstimatedUnInstallTime.ToString());
			writer.WriteAttributeString("uncmdarg", UnInstallCommandArgs);
			writer.WriteAttributeString("silentuninst", SilentUninstall);
			writer.WriteAttributeString("ugdesttime", EstimatedUpgradeTime);
			writer.WriteAttributeString("ugdcmdargs", UpgradeCommandArgs);
			writer.WriteAttributeString("silentugd", SilentUpgrade);
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

		string AddonVersion { get; }
		string AddonGroup { get; }
		string ClientType { get; }
		string Platform { get; }

		// install info
		int EstimatedInstallTime { get; }

		string InstallCommandArgs { get; }
		string SilentInstall { get; } // will not use

		// uninstall info
		int EstimatedUnInstallTime { get; }

		string UnInstallCommandArgs { get; }
		string SilentUninstall { get; } // will not use

		string EstimatedUpgradeTime { get; }
		string UpgradeCommandArgs { get; }
		string SilentUpgrade { get; }

		// parameter from SBO client
		string InstallPath { get; }

		string DllPath { get; }
	}
}