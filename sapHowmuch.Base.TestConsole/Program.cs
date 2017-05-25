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
						// SAP B1 클라이언트의 초기화면 (회사 연결 전) 에서는  MetadataAutoRefresh 가 false
						// 회사에 로그인 되면 true 가 됨.
						SapStream.UiApp.MetadataAutoRefresh = false;
						Console.WriteLine($"MetadataAutoRefresh after login: {SapStream.UiApp.MetadataAutoRefresh}");
					}
				}

				SapStream.AppEventStream.Subscribe(ev =>
				{
					Console.WriteLine($"[AppEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					Console.WriteLine($"EventType: {ev.DetailArg}");
				});

				SapStream.ItemStream.Subscribe(ev =>
				{
					Console.WriteLine($"[ItemEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					Console.WriteLine($"{ev.FormUid}, {ev.DetailArg.FormTypeEx}, {ev.DetailArg.ItemUID}, {ev.DetailArg.ColUID}");
					Console.WriteLine($"Mode: {ev.DetailArg.FormMode}");
					Console.WriteLine($"EventType: {ev.DetailArg.EventType} // {ev.DetailArg.BeforeAction} // {ev.DetailArg.ActionSuccess}");
					Console.WriteLine($"ItemChanged: {ev.DetailArg.ItemChanged}");
					Console.WriteLine($"Row: {ev.DetailArg.Row}");
					Console.WriteLine($"InnerEvent: {ev.DetailArg.InnerEvent}");
					Console.WriteLine($"Char: {ev.DetailArg.CharPressed} // Modifiers: {ev.DetailArg.Modifiers} // PopUpIndicator: {ev.DetailArg.PopUpIndicator}");
				});

				SapStream.MenuStream.Subscribe(ev =>
				{
					Console.WriteLine($"[MenuEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					Console.WriteLine($"{ev.DetailArg.BeforeAction}, {ev.DetailArg.MenuUID}, {ev.DetailArg.InnerEvent}");
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadLine();
		}
	}
}