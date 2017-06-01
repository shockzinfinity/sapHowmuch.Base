using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace sapHowmuch.Base.Helpers
{
	public static class MenuHelper
	{
		private static readonly List<AddonMenuEvent> _addonMenuEvents = new List<AddonMenuEvent>();
		private static bool _isInitialized;
		private static IDisposable _menuSubscribe;

		private static void Initialize()
		{
			if (!_isInitialized)
			{
				BindEvents();
				sapHowmuchLogger.Debug("MenuHelper.Initialized");
			}

			_isInitialized = true;
		}

		private static void BindEvents()
		{
			sapHowmuchLogger.Trace("MenuHelper.BindEvents");

			System.Windows.Forms.Application.ApplicationExit -= Application_ApplicationExit;
			System.Windows.Forms.Application.ApplicationExit += Application_ApplicationExit;

			if (_menuSubscribe != null)
				_menuSubscribe.Dispose();

			// TODO: menu subscribe 에 대한 before/after 정책결정 필요
			// e.g.) 코어쪽의 메뉴 호출 시에 before 에서 bubble event 를 true/false 해야 하는 경우가 생길 수 있음.
			// 이는 main stream 에서 조정할 필요가 있다.
			_menuSubscribe = SapStream.MenuEventStream.Where(x => !x.DetailArg.BeforeAction).Subscribe(x =>
			{
				//if (x.DetailArg.BeforeAction) return;

				var menuId = x.DetailArg.MenuUID;
				var menuEvent = _addonMenuEvents.FirstOrDefault(e => e.MenuId == menuId);

				if (menuEvent == null) return;

				if (menuEvent.ThreadedAction)
				{
					var thread = new Thread(() => menuEvent.Action())
					{
						Name = menuEvent.MenuId,
						IsBackground = true
					};

					thread.SetApartmentState(ApartmentState.STA);
					thread.Start();
				}
				else
				{
					menuEvent.Action();
				}
			});
		}

		private static void Application_ApplicationExit(object sender, EventArgs e)
		{
			foreach (var item in _addonMenuEvents)
			{
				SapStream.UiApp.Menus.RemoveEx(item.MenuId);
			}
		}

		public static void AddMenuItemEvent(string title, string menuId, string parentMenuId, Action action = null, int position = -1, bool threadedAction = false)
		{
			_addonMenuEvents.Add(new AddonMenuEvent
			{
				MenuId = menuId,
				ParentMenuId = parentMenuId,
				Action = action,
				ThreadedAction = threadedAction,
				Position = position
			});

			AddItem(title, menuId, parentMenuId, position);
			BindEvents();
		}

		public static SAPbouiCOM.MenuItem AddFolder(this SAPbouiCOM.MenuItem parentMenuItem, string title, string itemId, int position = -1)
		{
			return parentMenuItem.Add(title, itemId, position, SAPbouiCOM.BoMenuType.mt_POPUP);
		}

		public static SAPbouiCOM.MenuItem AddFolder(string title, string itemId, string parentItemId, int position = -1)
		{
			var parentMenuItem = SapStream.UiApp.Menus.Item(parentItemId);
			return parentMenuItem.Add(title, itemId, position, SAPbouiCOM.BoMenuType.mt_POPUP);
		}

		public static SAPbouiCOM.MenuItem AddItem(this SAPbouiCOM.MenuItem parentMenuItem, string title, string itemId, int position = -1)
		{
			return parentMenuItem.Add(title, itemId, position, SAPbouiCOM.BoMenuType.mt_STRING);
		}

		public static SAPbouiCOM.MenuItem AddItem(string title, string itemId, string parentItemId, int position = -1)
		{
			var parentMenuItem = SapStream.UiApp.Menus.Item(parentItemId);
			return parentMenuItem.Add(title, itemId, position, SAPbouiCOM.BoMenuType.mt_STRING);
		}

		public static SAPbouiCOM.MenuItem AddSeparator(this SAPbouiCOM.MenuItem parentMenuItem, string itemId)
		{
			return parentMenuItem.Add("", itemId, -1, SAPbouiCOM.BoMenuType.mt_SEPERATOR);
		}

		private static SAPbouiCOM.MenuItem Add(this SAPbouiCOM.MenuItem parentMenuItem, string title, string itemId, int position, SAPbouiCOM.BoMenuType type)
		{
			try
			{
				Initialize();

				if (!parentMenuItem.SubMenus.Exists(itemId))
				{
					var creationPackage = SapStream.UiApp.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams) as SAPbouiCOM.MenuCreationParams;

					creationPackage.Type = type;
					creationPackage.UniqueID = itemId;
					creationPackage.String = title;
					creationPackage.Image = "";
					creationPackage.Enabled = true;
					creationPackage.Position = position;

					parentMenuItem.SubMenus.AddEx(creationPackage);
				}
			}
			catch (Exception ex)
			{
				SapStream.UiApp.SetStatusBarMessage($"Error creating menu item (string) {itemId}: {ex.Message}");
			}

			try
			{
				if (type == SAPbouiCOM.BoMenuType.mt_POPUP)
					return parentMenuItem.SubMenus.Item(itemId);
				else
					return parentMenuItem;
			}
			catch (Exception ex)
			{
				throw new Exception($"Menu {itemId} not found in {parentMenuItem.UID}", ex);
			}
		}

		public static void LoadFromXML(string fileName)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(fileName);

			// TODO: 여러개의 Menus 가 있을 경우
			// 이 부분에서 메뉴 아이콘 정책이 필요
			var node = xmlDoc.SelectSingleNode("/Application/Menus/action/Menu");
			var imageAttr = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(a => a.Name == "Image");

			if (imageAttr != null && !string.IsNullOrWhiteSpace(imageAttr.Value))
			{
				imageAttr.Value = $"{Environment.CurrentDirectory}\\Resources\\{imageAttr.Value}";
				node.Attributes.SetNamedItem(imageAttr);
			}

			var tmpStr = xmlDoc.InnerXml;
			SapStream.UiApp.LoadBatchActions(ref tmpStr);
			var result = SapStream.UiApp.GetLastBatchResults();
		}

		public static void LoadFromXML(Assembly assembly)
		{
			var xmlDoc = new XmlDocument();

			using (var xmlStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Menu.xml"))
			{
				try
				{
					xmlDoc.Load(xmlStream);

					// TODO: 여러개의 Menus 가 있을 경우
					// 이 부분에서 메뉴 아이콘 정책이 필요
					var node = xmlDoc.SelectSingleNode("/Application/Menus/action/Menu");
					var imageAttr = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(a => a.Name == "Image");

					if (imageAttr != null && !string.IsNullOrWhiteSpace(imageAttr.Value))
					{
						imageAttr.Value = $"{Environment.CurrentDirectory}\\Resources\\{imageAttr.Value}";
						node.Attributes.SetNamedItem(imageAttr);
					}

					string menuXmlString = xmlDoc.InnerXml;
					string toRemove = menuXmlString.Replace("type=\"add\"", "type=\"remove\"");
					SapStream.UiApp.LoadBatchActions(ref toRemove);
					SapStream.UiApp.LoadBatchActions(ref menuXmlString);

					// TODO: addon menu event 적용
					// LoadBatchActions 로 메뉴를 등록하게 될 경우, 폼 오픈 이벤트에 대한 별도의 처리가 필요하다.
					sapHowmuchLogger.Debug(SapStream.UiApp.GetLastBatchResults());

					AddTerminateAppMenu(assembly);
				}
				catch (Exception ex)
				{
					sapHowmuchLogger.Error($"Failed to load menu: {ex.Message}");
					throw;
				}
			}
		}

		[Conditional("DEBUG")]
		private static void AddTerminateAppMenu(Assembly assembly)
		{
			if (SapStream.IsUiConnected)
			{
				var appNames = assembly.GetName().Name.Split('.');

				AddMenuItemEvent(
					$"{appNames[appNames.Length - 1]}종료",
					$"{appNames[appNames.Length - 1]}Stop",
					SboMenuItem.Modules,
					() =>
					{
						SapStream.UiApp.Menus.RemoveEx($"{appNames[appNames.Length - 1]}Stop");
						Environment.Exit(0);
					},
					-1)
			}
		}

		public static void LoadAndAddMenuItemsFromFormControllers(Assembly assembly)
		{
			var formControllers = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(FormController)) && t.GetInterfaces().Contains(typeof(IFormMenuItem))).ToList();

			foreach (var formController in formControllers)
			{
				var formMenuItem = Activator.CreateInstance(formController) as IFormMenuItem;

				foreach (var menuItem in formMenuItem.MenuItems)
				{
					var item = new AddonMenuEvent
					{
						MenuId = menuItem.MenuItemId,
						ParentMenuId = menuItem.ParentMenuItemId,
						Position = menuItem.MenuItemPosition,
						Title = menuItem.MenuItemTitle,
						Action = () =>
						{
							CreateOrStartController(formController);
						},
						ThreadedAction = false
					};

					AddMenuItemEvent(item.Title, item.MenuId, item.ParentMenuId, item.Action, item.Position);
				}
			}

			BindEvents();
		}

		private static readonly List<FormController> _formControllerInstances = new List<FormController>();

		private static void CreateOrStartController(Type formControllerType)
		{
			// clean up disposed form controllers
			_formControllerInstances.RemoveAll(i => i.Form == null);
			GC.Collect();

			var formController = _formControllerInstances.FirstOrDefault(i => i.GetType() == formControllerType && i.Unique);
			if (formController == null)
			{
				formController = (FormController)Activator.CreateInstance(formControllerType);
				_formControllerInstances.Add(formController);
			}
			formController.Start();
		}
	}

	public class AddonMenuEvent
	{
		public bool ThreadedAction { get; set; }

		public string ParentMenuId { get; set; }
		public string MenuId { get; set; }
		public Action Action { get; set; }
		public string Title { get; set; }

		/// <summary>
		/// Position, -1 = Last
		/// </summary>
		public int Position { get; set; }
	}
}