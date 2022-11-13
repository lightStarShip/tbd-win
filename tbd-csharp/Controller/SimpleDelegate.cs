using System;
using System.IO;
using System.Runtime.InteropServices;
using NLog;
using tbd.Properties;
using tbd.Util;
using Newtonsoft.Json;
using System.Windows.Forms;
using Microsoft.Win32;

namespace tbd.Controller
{
    public static class SimpleDelegate
    {
        public class ApiCmd
        {
            public int cmd;
            public static ApiCmd Parse(string json)
            {
                ApiCmd cmd = JsonConvert.DeserializeObject<ApiCmd>(json, new JsonSerializerSettings()
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                });

                return cmd;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate string UIAPI(string msg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackLog(string msg);

        public static Wallet wallet;
        public static Stripe stripe;

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private const string DLLNAME = "libsimple.dll";
        private static CallBackLog simpleLogCB = new CallBackLog(LogFunc);
        private static UIAPI simpleCallbackAPI = new UIAPI(ApiFunc);
        public static string CmdLine = "set ALL_PROXY=socks5://127.0.0.1:31080";
        private static string exceptionStr = "<local>;localhost;127.*;10.*;172.16.*;172.17.*;172.18.*;" +
            "172.19.*;172.20.*;172.21.*;172.22.*;172.23.*;172.24.*;172.25.*;172.26.*;172.27.*;172.28.*;" +
            "172.29.*;172.30.*;172.31.*;192.168.*";
        static SimpleDelegate()
        {
            string dllPath = Utils.GetAppDataPath(DLLNAME);
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

        #region Simple Protocol Lib

        public static void InitLib()
        {            
#if DEBUG
            InitLibWin(1, 0, "https://lightstarship.github.io", ref simpleCallbackAPI, ref simpleLogCB);
#else
        InitLibWin(0, 1, "https://lightstarship.github.io", ref simpleCallbackAPI, ref simpleLogCB);
#endif
            wallet = Wallet.LoadWallet();
            if (wallet.Address != null)
            {
                stripe = Stripe.LoadStripe(wallet.Address);
                Node.LoadNodeList();
            }
        }

        public static void ReloadWallet()
        {
            wallet = Wallet.LoadWallet();
            if (wallet.Address != null)
            {
                stripe = Stripe.LoadStripe(wallet.Address);
                Node.LoadNodeList(true);
            }
        }

        public static bool HasWallet()
        {
            return wallet.Address != null && wallet.Address.Length > 0;
        }
        public static bool WalletIsOpen()
        {
            return LibIsOpen() != 0;
        }

        public static bool StartSimpleProtocol()
        {
            string lclProxy = $"{ProxyIP}:{ProxyPort}";

            string addr = stripe.currentNode;
            if (addr == null)
            {
                return false;
            }
            Node node = Node.NodeCache[addr];
            if (node == null)
            {
                return false;
            }
            return StartProxyWin(lclProxy, node.Host, node.NodeAddr);
        }
       

       public static string ApiFunc(string msg)
        {
            ApiCmd cmd = ApiCmd.Parse(msg);
            switch (cmd.cmd)
            {
                case 1:
                    //Console.WriteLine($"======ApiFunc1================>>>>{Resources.dns_conf}");
                    return Resources.dns_conf;
                case 2:
                    //Console.WriteLine($"======ApiFunc2================>>>>{Resources.bypass}");
                    return Resources.bypass;
                default:
                    return "";
            }
        }

        public static void LogFunc(string log)
        {
            Console.WriteLine(log);
            logger.Info(log);
        }

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern void PrintBye();

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern byte LibIsOpen();

        [DllImport(DLLNAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr LibWalletAddress();

       [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void InitLibWin(byte debug, byte logLevel, string confUrl,
            [MarshalAs(UnmanagedType.FunctionPtr)] ref UIAPI api,
            [MarshalAs(UnmanagedType.FunctionPtr)] ref CallBackLog cb);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool LoadWalletWin(string walletData, string pwd);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool OpenWalletWin(string walletData, string pwd);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool StartProxyWin(string localProxy, string nodeIP, string nodeID);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern IntPtr StripeBasic(string wAddr, string cusID);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern float BalanceWin(Int64 expireDay);
        

        [DllImport(DLLNAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern void StopProxy();

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr NodeConfigData();

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern float PingValWin(string addr, string ip);
        
        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern IntPtr ChangeSrvWin(string addr, string ip);
        
        #endregion

        #region system proxy settings
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption
          (IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        public const string ProxyIP = "127.0.0.1";
        public const short ProxyPort = 31080;
        public const short PirvoxyPort = 31081;
        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        };
        //
        public static bool SetSysProxy(bool on)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey
               ("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            bool settingsReturn, refreshReturn;
            if (on){ 
                registry.SetValue("ProxyEnable", 1);
                registry.SetValue("ProxyServer", $"http://localhost:{PirvoxyPort}");
                registry.SetValue("ProxyOverride", exceptionStr);
                if ((int)registry.GetValue("ProxyEnable", 0) == 0)
                {
                    return false;
                }
            }
            else
            {
                registry.SetValue("ProxyEnable", 0);
                registry.SetValue("ProxyServer", 0);
                registry.SetValue("ProxyOverride", "");
                if ((int)registry.GetValue("ProxyEnable", 1) == 1)
                {
                    return false;
                }
            }
            registry.Close();
            settingsReturn = InternetSetOption
            (IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption
            (IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
            Console.WriteLine($"------>>>return values settingsReturn={settingsReturn} refreshReturn={refreshReturn}");
            return refreshReturn && settingsReturn;
        }
        
        public static bool IsProxySet()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey
               ("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            return (int)registry.GetValue("ProxyEnable", 1) == 1;
        }

        #endregion
    }
}