using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace sapHowmuch.VSExtension.Utilities
{
	partial class LocateFile
	{
		[ComImport, Guid("000214F2-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IEnumIDList
		{
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int Next(uint celt, IntPtr rgelt, out uint pceltFetched);

			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int Skip([In] uint celt);

			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int Reset();

			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int Clone([MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenum);
		}
	}
}