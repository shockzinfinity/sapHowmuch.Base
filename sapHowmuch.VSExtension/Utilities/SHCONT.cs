﻿using System;

namespace sapHowmuch.VSExtension.Utilities
{
	partial class LocateFile
	{
		[Flags]
		internal enum SHCONT : ushort
		{
			SHCONTF_CHECKING_FOR_CHILDREN = 0x10,
			SHCONTF_ENABLE_ASYNC = 0x8000,
			SHCONTF_FASTITEMS = 0x2000,
			SHCONTF_FLATLIST = 0x4000,
			SHCONTF_FOLDERS = 0x20,
			SHCONTF_INCLUDEHIDDEN = 0x80,
			SHCONTF_INIT_ON_FIRST_NEXT = 0x100,
			SHCONTF_NAVIGATION_ENUM = 0x1000,
			SHCONTF_NETPRINTERSRCH = 0x200,
			SHCONTF_NONFOLDERS = 0x40,
			SHCONTF_SHAREABLE = 0x400,
			SHCONTF_STORAGE = 0x800
		}
	}
}