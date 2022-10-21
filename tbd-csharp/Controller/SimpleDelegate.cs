using System;
using System.IO;
using System.Runtime.InteropServices;
using NLog;
using tbd.Controller;
using tbd.Properties;
using tbd.Util;

public static class SimpleDelegate
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate string UIAPI(string msg);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CallBackLog(string msg);

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
        Console.WriteLine($"======2================>>>>");
        IntPtr urlPtr = Marshal.StringToHGlobalAnsi("test");
        CallBackLog logcb = new CallBackLog(LogFunc);
        UIAPI api = new UIAPI(ApiFunc);
        InitLibWin(1, 2, "test", ref api, ref logcb);
        Console.WriteLine($"======3================>>>>");
    }

    public static string ApiFunc(string msg)
    {
        Console.WriteLine($"======ApiFunc================>>>>{msg}");
        return "test from charp$$$$$$$";
    }

    public static void LogFunc(string log)
    {
        logger.Info(log);
    }


    [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern void PrintBye();

    [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern byte LibIsOpen();

    [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    public static extern void InitLibWin(byte debug,byte logLevel, string confUrl,
        [MarshalAs(UnmanagedType.FunctionPtr)] ref UIAPI api,
        [MarshalAs(UnmanagedType.FunctionPtr)] ref CallBackLog cb);
}
