using Autofac;
using System;

namespace sapHowmuch.Base.TestConsole
{
	internal class Program
	{
		private static IContainer _container;

		private static void Main(string[] args)
		{
			try
			{
				_container = DependencyConfig.Configure();

				var sapStream = _container.Resolve<ISapStream>();

				Console.WriteLine(sapStream.DICompany.CompanyName);
				Console.WriteLine("Is connect to ui app? {0}", sapStream.IsConnectToUI);
				Console.WriteLine($"Cookie: {sapStream.DICompany.GetContextCookie()}");

				if (sapStream.IsConnectToUI)
				{
					Console.WriteLine("IsHostedEnvironment: {0}", sapStream.UiApp.IsHostedEnvironment);
					// NOTE: Company 에 로그인 되면 MetadataAutoRefresh 가 true 가 됨.
					Console.WriteLine("MetadataAutoRefresh: {0}", sapStream.UiApp.MetadataAutoRefresh);

					if (sapStream.UiApp.MetadataAutoRefresh)
					{
						// SAP B1 클라이언트의 초기화면 (회사 연결 전) 에서는  MetadataAutoRefresh 가 false
						// 회사에 로그인 되면 true 가 됨.
						sapStream.UiApp.MetadataAutoRefresh = false;
						Console.WriteLine($"MetadataAutoRefresh after login: {sapStream.UiApp.MetadataAutoRefresh}");
					}

					sapStream.AppEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[AppEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
						Console.WriteLine($"EventType: {ev.DetailArg}");
					});

					sapStream.ItemEventStream.Subscribe(ev =>
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

					sapStream.StatusBarEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[StatusBarEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
						Console.WriteLine($"{ev.Message}");
					});

					sapStream.FormDataEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[FormDataEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
						Console.WriteLine($"{ev.DetailArg.EventType}");
					});

					sapStream.MenuEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[MenuEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
						Console.WriteLine($"{ev.DetailArg.BeforeAction}, {ev.DetailArg.MenuUID}, {ev.DetailArg.InnerEvent}");
					});

					sapStream.PrintEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[PrintEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					sapStream.ProgressBarEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[PrgressBarEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					sapStream.ReportDataInfoEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[ReportDataInfoEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					sapStream.RightClickEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[RightClickEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					sapStream.ServerInvokeCompletedEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[ServerInvokeCompletedEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					sapStream.UDOEventStream.Subscribe(ev =>
					{
						Console.WriteLine($"[UDOEvent] {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
					});

					sapStream.WidgetEventStream.Subscribe(ev =>
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