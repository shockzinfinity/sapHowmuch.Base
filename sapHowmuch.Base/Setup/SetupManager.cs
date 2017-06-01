using sapHowmuch.Base.Helpers;
using sapHowmuch.Base.Services;
using System;
using System.Linq;
using System.Reflection;

namespace sapHowmuch.Base.Setup
{
	public static class SetupManager
	{
		public static void FindAndRunSetups(Assembly assembly)
		{
			var setups = assembly.GetTypes().Where(t => !t.IsInterface && t.GetInterfaces().Contains(typeof(ISetup))).ToArray();

			foreach (var setupInstance in setups.Select(s => Activator.CreateInstance(s) as ISetup))
			{
				RunSetup(setupInstance);
			}
		}

		public static void RunSetup<TSetup>(TSetup setupInstance) where TSetup : ISetup
		{
			var setup = setupInstance.GetType();
			var setupclassName = setup.Name.Replace("Setup", string.Empty);
			var key = $"setup.lv.{setupclassName}";

			if (key.Length > 30)
				sapHowmuchLogger.Warn($"Setup class '{setupclassName}' Name is too long (max 30, actual {setupclassName.Length}");

			key = key.Substring(0, 30);

			var lastVersionInstalled = SettingService.Instance.GetSettingByKey(key, 0);

			if (lastVersionInstalled < setupInstance.Version)
			{
				try
				{
					sapHowmuchLogger.Info($"Running setup for {setup.Name}, current version is {lastVersionInstalled}, new version is {setupInstance.Version})");

					setupInstance.Run();

					SettingService.Instance.SaveSetting(key, setupInstance.Version);
				}
				catch (Exception ex)
				{
					sapHowmuchLogger.Error($"Setup error in {setup.Name}: {ex.Message}");
					throw;
				}
			}

			sapHowmuchLogger.Info($"Setup for {setup.Name} is up to date (v.{setupInstance.Version})");
		}
	}
}