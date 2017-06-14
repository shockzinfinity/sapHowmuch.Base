﻿//------------------------------------------------------------------------------
// <copyright file="LocateFolderCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using EnvDTE80;
using sapHowmuch.VSExtension.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace sapHowmuch.VSExtension
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class LocateFolderCommand
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0100;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("f6d5675a-6ea6-461f-9e1f-7563cd6044a0");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly Package package;

		/// <summary>
		/// Initializes a new instance of the <see cref="LocateFolderCommand"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		private LocateFolderCommand(Package package)
		{
			if (package == null)
			{
				throw new ArgumentNullException(nameof(package));
			}

			this.package = package;

			OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (commandService != null)
			{
				var menuCommandID = new CommandID(CommandSet, CommandId);
				var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
				commandService.AddCommand(menuItem);
			}
		}

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static LocateFolderCommand Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private IServiceProvider ServiceProvider
		{
			get
			{
				return this.package;
			}
		}

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static void Initialize(Package package)
		{
			Instance = new LocateFolderCommand(package);
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked.
		/// See the constructor to see how the menu item is associated with this function using
		/// OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void MenuItemCallback(object sender, EventArgs e)
		{
			//string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
			//string title = "LocateFolderCommand";

			//// Show a message box to prove we were here
			//VsShellUtilities.ShowMessageBox(
			//	this.ServiceProvider,
			//	message,
			//	title,
			//	OLEMSGICON.OLEMSGICON_INFO,
			//	OLEMSGBUTTON.OLEMSGBUTTON_OK,
			//	OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

			var selectedItems = ((UIHierarchy)((DTE2)this.ServiceProvider.GetService(typeof(DTE))).Windows.Item("{3AE79031-E1BC-11D0-8F78-00A0C9110057}").Object).SelectedItems as object[];

			if (selectedItems != null)
			{
				LocateFile.FilesOrFolders((IEnumerable<string>)selectedItems.Where(t => (t as UIHierarchyItem)?.Object is ProjectItem).Select(t => (((ProjectItem)((UIHierarchyItem)t).Object).FileNames[1])));
			}
		}
	}
}
