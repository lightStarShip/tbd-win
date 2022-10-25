﻿using System;
using System.Runtime.InteropServices;

public class DllUtils
{
	public DllUtils()
	{
	}
    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibrary(string path);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibraryA(string path);

    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr GetModuleHandle(String moduleName);
}