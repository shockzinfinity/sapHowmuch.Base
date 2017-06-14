using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace sapHowmuch.VSExtension.Utilities
{
	internal static partial class LocateFile
	{
		private static Guid IID_IShellFolder = typeof(IShellFolder).GUID;
		private static int pointerSize = Marshal.SizeOf(typeof(IntPtr));

		public static void FileOrFolder(string path, bool edit = false)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			IntPtr pidlFolder = PathToAbsolutePIDL(path);

			try
			{
				SHOpenFolderAndSelectItems(pidlFolder, null, edit);
			}
			finally
			{
				NativeMethods.ILFree(pidlFolder);
			}
		}

		public static void FilesOrFolders(IEnumerable<FileSystemInfo> paths)
		{
			if (paths == null)
			{
				throw new ArgumentNullException(nameof(paths));
			}

			if (paths.Count<FileSystemInfo>() != 0)
			{
				foreach (IGrouping<string, FileSystemInfo> grouping in from p in paths group p by Path.GetDirectoryName(p.FullName))
				{
					FilesOrFolders(Path.GetDirectoryName(grouping.First<FileSystemInfo>().FullName), (from fsi in grouping select fsi.Name).ToList<string>());
				}
			}
		}

		public static void FilesOrFolders(IEnumerable<string> paths)
		{
			FilesOrFolders(PathToFileSystemInfo(paths));
		}

		public static void FilesOrFolders(params string[] paths)
		{
			FilesOrFolders((IEnumerable<string>)paths);
		}

		public static void FilesOrFolders(string parentDirectory, ICollection<string> filenames)
		{
			if (filenames == null)
			{
				throw new ArgumentNullException(nameof(filenames));
			}

			if (filenames.Count != 0)
			{
				IntPtr pidl = PathToAbsolutePIDL(parentDirectory);
				try
				{
					IShellFolder parentFolder = PIDLToShellFolder(pidl);
					List<IntPtr> list = new List<IntPtr>(filenames.Count);

					foreach (string str in filenames)
					{
						list.Add(GetShellFolderChildrenRelativePIDL(parentFolder, str));
					}

					try
					{
						SHOpenFolderAndSelectItems(pidl, list.ToArray(), false);
					}
					finally
					{
						using (List<IntPtr>.Enumerator enumerator2 = list.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								NativeMethods.ILFree(enumerator2.Current);
							}
						}
					}
				}
				finally
				{
					NativeMethods.ILFree(pidl);
				}
			}
		}

		private static IntPtr GetShellFolderChildrenRelativePIDL(IShellFolder parentFolder, string displayName)
		{
			uint num;
			IntPtr ptr;
			NativeMethods.CreateBindCtx();
			parentFolder.ParseDisplayName(IntPtr.Zero, null, displayName, out num, out ptr, 0);
			return ptr;
		}

		private static IntPtr PathToAbsolutePIDL(string path) => GetShellFolderChildrenRelativePIDL(NativeMethods.SHGetDesktopFolder(), path);

		private static IEnumerable<FileSystemInfo> PathToFileSystemInfo(IEnumerable<string> paths)
		{
			foreach (string iteratorVariable0 in paths)
			{
				string path = iteratorVariable0;

				if (path.EndsWith(Path.DirectorySeparatorChar.ToString()) || path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
				{
					path = path.Remove(path.Length - 1);
				}

				if (Directory.Exists(path))
				{
					yield return new DirectoryInfo(path);
				}
				else
				{
					if (!File.Exists(path))
					{
						throw new FileNotFoundException("The specified file or folder doesn't exists : " + path, path);
					}

					yield return new FileInfo(path);
				}
			}
		}

		private static IShellFolder PIDLToShellFolder(IntPtr pidl) => PIDLToShellFolder(NativeMethods.SHGetDesktopFolder(), pidl);

		private static IShellFolder PIDLToShellFolder(IShellFolder parent, IntPtr pidl)
		{
			IShellFolder folder;
			Marshal.ThrowExceptionForHR(parent.BindToObject(pidl, null, ref IID_IShellFolder, out folder));
			return folder;
		}

		private static void SHOpenFolderAndSelectItems(IntPtr pidlFolder, IntPtr[] apidl, bool edit)
		{
			NativeMethods.SHOpenFolderAndSelectItems(pidlFolder, apidl, edit ? 1 : 0);
		}
	}
}