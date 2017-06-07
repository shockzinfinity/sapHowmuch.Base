using sapHowmuch.Base.Constants;
using sapHowmuch.Base.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace sapHowmuch.Base.Helpers
{
	public static class MenuHelper
	{
		private static readonly List<FormController> _formControllerInstances = new List<FormController>();
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
				if (SapStream.UiApp.Menus.Exists(item.MenuId))
				{
					SapStream.UiApp.Menus.RemoveEx(item.MenuId);
				}
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

		public static void LoadFromXML(Assembly assembly)
		{
			var xmlDoc = new XmlDocument();

			using (var xmlStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Menu.xml"))
			{
				try
				{
					xmlDoc.Load(xmlStream);

					var menuNodeList = xmlDoc.GetElementsByTagName("Menu").Cast<XmlNode>();

					// menu.xml 내의 unique id 중복체크
					if (menuNodeList.GroupBy(x => x.Attributes["UniqueID"]).Any(g => g.Count() > 1))
					{
						throw new Exception("duplicate unique id");
					}

					var formControllers = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(FormController))).ToList();

					foreach (XmlNode node in menuNodeList)
					{
						var attrs = node.Attributes.Cast<XmlAttribute>();

						var imageAttr = attrs.FirstOrDefault(a => a.Name == "Image");

						if (imageAttr != null && !string.IsNullOrWhiteSpace(imageAttr.Value))
						{
							//imageAttr.Value = $"{Environment.CurrentDirectory}\\Resources\\{imageAttr.Value}";
							imageAttr.Value = Path.Combine(Environment.CurrentDirectory, "Resources", imageAttr.Value);
							node.Attributes.SetNamedItem(imageAttr);
						}

						_addonMenuEvents.Add(GetEventFromAttributes(assembly, attrs));
					}

					BindEvents();

					string menuXmlString = xmlDoc.InnerXml;
					string toRemove = menuXmlString.Replace("type=\"add\"", "type=\"remove\"");

					SapStream.UiApp.LoadBatchActions(ref toRemove);
					sapHowmuchLogger.Debug(SapStream.UiApp.GetLastBatchResults());

					SapStream.UiApp.LoadBatchActions(ref menuXmlString);
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

		private static AddonMenuEvent GetEventFromAttributes(Assembly entryAssembly, IEnumerable<XmlAttribute> attrs)
		{
			// TODO: menu type 이 화면인 경우만 등록하도록 변경 필요

			var menuId = attrs.FirstOrDefault(a => a.Name == "UniqueID").Value;
			var parentMenuId = attrs.FirstOrDefault(a => a.Name == "FatherUID").Value;
			var position = Convert.ToInt32(attrs.FirstOrDefault(a => a.Name == "Position").Value);
			var title = attrs.FirstOrDefault(a => a.Name == "String").Value;

			Action formAction = null;

			if (attrs.FirstOrDefault(a => a.Name == "FormType") != null)
			{
				var formType = entryAssembly.GetType(attrs.FirstOrDefault(a => a.Name == "FormType").Value);
				formAction = () => CreateOrStartController(formType);
			}
			else
			{
				// TODO: form type 어트리뷰트가 존재하지 않을 경우의 처리
			}

			var threadedAction = attrs.FirstOrDefault(a => a.Name == "ThreadedAction") == null ? false : Convert.ToBoolean(attrs.FirstOrDefault(a => a.Name == "ThreadedAction").Value);

			return new AddonMenuEvent()
			{
				MenuId = menuId,
				ParentMenuId = parentMenuId,
				Position = position,
				Title = title,
				Action = formAction,
				ThreadedAction = threadedAction
			};
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
						//Environment.Exit(0);
						System.Windows.Forms.Application.Exit();
					},
					-1);
			}
		}

		private static void CreateOrStartController(Type formControllerType)
		{
			if (formControllerType == null)
				throw new ArgumentNullException(nameof(formControllerType));

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