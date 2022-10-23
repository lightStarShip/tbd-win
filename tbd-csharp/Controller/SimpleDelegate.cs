using System;
using System.IO;
using System.Runtime.InteropServices;
using NLog;
using tbd.Properties;
using tbd.Util;
using Newtonsoft.Json;
namespace tbd.Controller
{
    public static class SimpleDelegate
    {
        public class Wallet
        {
            [JsonIgnore]
            public string Address;
            public string RawData;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate string UIAPI(string msg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackLog(string msg);

        public static Wallet wallet;

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private const string DLLNAME = "libsimple.dll";
        private static readonly string Wallet_FILE = "wallet.json";

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
                
                LoadWallet();
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

        private static void LoadWallet()
        {
            if (File.Exists(Wallet_FILE))
            {
                string content = File.ReadAllText(Wallet_FILE);
                wallet = JsonConvert.DeserializeObject<Wallet>(content, new JsonSerializerSettings()
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                });
                return;
            }
            wallet = new Wallet();
        }

        public static void InitLib()
        {

            CallBackLog logcb = new CallBackLog(LogFunc);
            UIAPI api = new UIAPI(ApiFunc);
#if DEBUG
            InitLibWin(1, 0, "https://lightstarship.github.io", ref api, ref logcb);
#else
        InitLibWin(0, 1, "https://lightstarship.github.io", ref api, ref logcb);
#endif
        }

        public static bool HasWallet()
        {
            return wallet.Address != null && wallet.Address.Length > 0;
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
        public static extern void InitLibWin(byte debug, byte logLevel, string confUrl,
            [MarshalAs(UnmanagedType.FunctionPtr)] ref UIAPI api,
            [MarshalAs(UnmanagedType.FunctionPtr)] ref CallBackLog cb);
    }
}