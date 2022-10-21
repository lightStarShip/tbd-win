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

            IntPtr dllHandle = DllUtils.LoadLibrary(dllPath);
            if (dllHandle == IntPtr.Zero)
            {
                Console.WriteLine($"---------------------->{ Marshal.GetLastWin32Error()}");
            }
        }
        catch (IOException e)
        {
            logger.LogUsefulException(e);
        }
        catch (System.Exception e)
        {
            logger.LogUsefulException(e);
        }
    }

    public static void TestLib()
    {
        Console.WriteLine($"======1================>>>>{LibIsOpen()}");
        PrintBye();
        Console.WriteLine($"---------------------->{ Marshal.GetLastWin32Error()}");
        Console.WriteLine($"======2================>>>>");
    }
    
    [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern void PrintBye();

    [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern byte LibIsOpen();
}
