using System;
using System.Diagnostics;
using System.Management;

namespace sapHowmuch.Base.TestConsole
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				Console.WriteLine(SapStream.DICompany.CompanyName);

				//var sapProcess = ProcessHelper.ByName("SAP Business One.exe");

				//foreach (var item in sapProcess)
				//{
				//	Console.WriteLine(item.Name);
				//}
				Console.WriteLine("Is connect to ui app? {0}", SapStream.IsConnectToUI);

				if (SapStream.IsConnectToUI)
				{
					Console.WriteLine("IsHostedEnvironment: {0}", SapStream.UiApp.IsHostedEnvironment);
					// NOTE: Company 에 로그인 되면 MetadataAutoRefresh 가 true 가 됨.
					Console.WriteLine("MetadataAutoRefresh: {0}", SapStream.UiApp.MetadataAutoRefresh);

					if (SapStream.UiApp.MetadataAutoRefresh)
					{
						SapStream.UiApp.MetadataAutoRefresh = false;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadLine();
		}
	}
}