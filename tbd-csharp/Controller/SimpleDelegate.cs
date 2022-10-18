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
            FileManager.UncompressFile(dllPath, Resources.libsscrypto_dll);
        }
        catch (IOException)
        {
        }
        catch (System.Exception e)
        {
            logger.LogUsefulException(e);
        }
        LoadLibrary(dllPath);
    }


    [DllImport("Kernel32.dll")]
    private static extern IntPtr LoadLibrary(string path);

    [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern void StopProxy();
}
