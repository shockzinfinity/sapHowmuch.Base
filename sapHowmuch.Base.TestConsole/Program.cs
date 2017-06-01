using Autofac;
using System;

namespace sapHowmuch.Base.TestConsole
{
	internal class Program
	{
		private static IContainer _container;

		[STAThread]
		private static void Main(string[] args)
		{
			try
			{
				_container = DependencyConfig.Configure();

				Console.WriteLine(SapStream.DICompany.CompanyName);
				Console.WriteLine("Is connect to ui app? {0}", SapStream.IsUiConnected);
				Console.WriteLine($"Cookie: {SapStream.DICompany.GetContextCookie()}");

				if (!SapStream.IsUiConnected)
				{
					SapStream.ConnectByUI();
				}

				if (SapStream.IsUiConnected)
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

					SapStream.AppEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[AppEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
						Console.WriteLine($"EventType: {ev.DetailArg}");
					});

					SapStream.ItemEventStream.Subscribe(ev =>
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

					SapStream.StatusBarEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[StatusBarEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
						Console.WriteLine($"{ev.Message}");
					});

					SapStream.FormDataEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[FormDataEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
						Console.WriteLine($"{ev.DetailArg.EventType}");
					});

					SapStream.MenuEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[MenuEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
						Console.WriteLine($"{ev.DetailArg.BeforeAction}, {ev.DetailArg.MenuUID}, {ev.DetailArg.InnerEvent}");
					});

					SapStream.PrintEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[PrintEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					SapStream.ProgressBarEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[PrgressBarEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					SapStream.ReportDataInfoEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[ReportDataInfoEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					SapStream.RightClickEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[RightClickEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					SapStream.ServerInvokeCompletedEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[ServerInvokeCompletedEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					SapStream.UDOEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[UDOEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					SapStream.WidgetEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[WidgetEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});
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