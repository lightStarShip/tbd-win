using System;
using System.IO;
using System.Runtime.InteropServices;
using NLog;
using tbd.Controller;
using tbd.Properties;
using tbd.Util;

public static class SimpleDelegate
{
    private static Logger logger = LogManager.GetCurrentClassLogger();
    private const string DLLNAME = "libsimple.dll";
	static SimpleDelegate()
    {
        string dllPath = Utils.GetTempPath(DLLNAME);
        try
        {
            FileManager.UncompressFile(dllPath, Resources.libsimple_dll);
        }
        catch (IOException)
        {
        }
        catch (System.Exception e)
        {
            logger.LogUsefulException(e);
        }
        IntPtr dllHandle = LoadLibrary(dllPath);
        if (dllHandle == IntPtr.Zero)
            Console.WriteLine($"==============>{ Marshal.GetLastWin32Error().ToString()}");
    }

    public static void StopP()
    {
        StopProxy();
    }

    [DllImport("Kernel32.dll")]
    private static extern IntPtr LoadLibrary(string path);

    [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
    public static extern void StopProxy();
}
