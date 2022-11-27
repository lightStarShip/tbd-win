using System;
using System.Runtime.InteropServices;

public class DllUtils
{
	public DllUtils()
	{
	}
    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibrary(string path);


    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibraryA(string path);

    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr GetModuleHandle(String moduleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetDllDirectory(string path);
}
