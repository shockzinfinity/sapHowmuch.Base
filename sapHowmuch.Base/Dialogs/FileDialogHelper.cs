using sapHowmuch.Base.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace sapHowmuch.Base.Dialogs
{
	public static class FileDialogHelper
	{
		public static string SaveFile(string filter = null, string defaultFileName = null, bool ignoreCancel = false)
		{
			var fileSelector = new SaveFileDialog();

			if (string.IsNullOrWhiteSpace(defaultFileName))
			{
				fileSelector.FileName = defaultFileName;
			}

			if (!string.IsNullOrWhiteSpace(filter))
			{
				fileSelector.Filter = filter;
			}

			var result = new STAInvoker<SaveFileDialog, DialogResult>(fileSelector, (x) => x.ShowDialog(ForegroundWindowWrapper.GetWindow())).Invoke();

			if (result != DialogResult.OK && !ignoreCancel)
				throw new DialogCanceledException();

			return fileSelector.FileName;
		}

		public static string OpenFile(string filter = null, string defaultFileName = null, bool ignoreCancel = false)
		{
			var fileSelector = new OpenFileDialog();

			if (string.IsNullOrWhiteSpace(defaultFileName))
			{
				fileSelector.FileName = defaultFileName;
			}

			if (!string.IsNullOrWhiteSpace(filter))
			{
				fileSelector.Filter = filter;
			}

			var result = new STAInvoker<OpenFileDialog, DialogResult>(fileSelector, (x) => x.ShowDialog(ForegroundWindowWrapper.GetWindow())).Invoke();

			if (result != DialogResult.OK && !ignoreCancel)
				throw new DialogCanceledException();

			return fileSelector.FileName;
		}

		public static string FolderBrowser(string defaultFolder = null, bool ignoreCancel = false)
		{
			var folderBrowserDialog = new FolderBrowserDialog();

			if (string.IsNullOrWhiteSpace(defaultFolder))
			{
				folderBrowserDialog.SelectedPath = defaultFolder;
			}

			var result = new STAInvoker<FolderBrowserDialog, DialogResult>(folderBrowserDialog, (x) => x.ShowDialog(ForegroundWindowWrapper.GetWindow())).Invoke();

			if (result != DialogResult.OK && !ignoreCancel)
				throw new DialogCanceledException();

			return folderBrowserDialog.SelectedPath;
		}
	}

	public class DialogCanceledException : Exception
	{
		public DialogCanceledException() : base("Dialog canceled")
		{
		}
	}

	internal class ForegroundWindowWrapper : IWin32Window
	{
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		public virtual IntPtr Handle { get; }

		public ForegroundWindowWrapper(IntPtr handle)
		{
			this.Handle = handle;
		}

		public static ForegroundWindowWrapper GetWindow()
		{
			return new ForegroundWindowWrapper(GetForegroundWindow());
		}
	}
}