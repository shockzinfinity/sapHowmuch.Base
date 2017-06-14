using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace sapHowmuch.VSExtension.Utilities
{
	partial class LocateFile
	{
		private class NativeMethods
		{
			private static readonly int pointerSize = Marshal.SizeOf(typeof(IntPtr));

			public static IBindCtx CreateBindCtx()
			{
				IBindCtx ctx;
				Marshal.ThrowExceptionForHR(CreateBindCtx_(0, out ctx));
				return ctx;
			}

			[DllImport("ole32.dll", EntryPoint = "CreateBindCtx")]
			public static extern int CreateBindCtx_(int reserved, out IBindCtx ppbc);

			[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
			public static extern IntPtr ILCreateFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath);

			[DllImport("shell32.dll")]
			public static extern void ILFree([In] IntPtr pidl);

			public static IShellFolder SHGetDesktopFolder()
			{
				IShellFolder folder;
				Marshal.ThrowExceptionForHR(SHGetDesktopFolder_(out folder));
				return folder;
			}

			[DllImport("shell32.dll", EntryPoint = "SHGetDesktopFolder", CharSet = CharSet.Unicode, SetLastError = true)]
			private static extern int SHGetDesktopFolder_([MarshalAs(UnmanagedType.Interface)] out IShellFolder ppshf);

			public static void SHOpenFolderAndSelectItems(IntPtr pidlFolder, IntPtr[] apidl, int dwFlags)
			{
				uint cidl = (apidl != null) ? ((uint)apidl.Length) : 0;
				Marshal.ThrowExceptionForHR(SHOpenFolderAndSelectItems_(pidlFolder, cidl, apidl, dwFlags));
			}

			[DllImport("shell32.dll", EntryPoint = "SHOpenFolderAndSelectItems")]
			private static extern int SHOpenFolderAndSelectItems_([In] IntPtr pidlFoldere, uint cidl, [In, Optional, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, int dwFlags);
		}
	}
}