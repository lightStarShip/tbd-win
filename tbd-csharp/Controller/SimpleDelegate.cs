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
        public class Wallet
        {
            [JsonIgnore]
            public string Address;
            public string RawData;
            public string Pwd;
            public void SaveToDisk(string data, string pwd)
            {
                this.RawData = data;
                this.Pwd = pwd;

                FileStream wFileStream = null;
                StreamWriter wStreamWriter = null;
                try
                {
                    wFileStream = File.Open(Wallet_FILE, FileMode.Create);
                    wStreamWriter = new StreamWriter(wFileStream);
                    var jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
                    wStreamWriter.Write(jsonString);
                    wStreamWriter.Flush();
                }
                catch (Exception e)
                {
                    logger.LogUsefulException(e);
                }
                finally
                {
                    if (wStreamWriter != null)
                        wStreamWriter.Dispose();
                    if (wFileStream != null)
                        wFileStream.Dispose();
                }
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate string UIAPI(string msg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void CallBackLog(string msg);

        public static Wallet wallet;

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private const string DLLNAME = "libsimple.dll";
        private static readonly string Wallet_FILE = "wallet.json";
        private static CallBackLog logcb = new CallBackLog(LogFunc);
        private static UIAPI api = new UIAPI(ApiFunc);

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

        #region Simple Protocol Lib

        private static void LoadWallet()
        {
            if (false == File.Exists(Wallet_FILE))
            {
                wallet = new Wallet();
                return;
            }

            string content = File.ReadAllText(Wallet_FILE);
            wallet = JsonConvert.DeserializeObject<Wallet>(content, new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });
            if (true == OpenWalletWin(wallet.RawData, wallet.Pwd))
            {
                IntPtr wPtr = LibWalletAddress();;
                wallet.Address = Marshal.PtrToStringAnsi(wPtr);
                wallet.Pwd = "";
                Console.WriteLine($"================>>>{wallet.Address}");
                return;
            }

            MessageBox.Show("Failed Open Wallet", "Error");
            wallet = new Wallet();
        }

        public static void InitLib()
        {            
#if DEBUG
            InitLibWin(1, 0, "https://lightstarship.github.io", ref api, ref logcb);
#else
        InitLibWin(0, 1, "https://lightstarship.github.io", ref api, ref logcb);
#endif
            LoadWallet();
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
            return StartProxyWin(lclProxy, "45.77.104.235", "SVEG15KMztpcmACzrr9fftZMta8Hcq3wv37nzayiggNNKk");
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

        [DllImport(DLLNAME, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern void StopProxy();
        #endregion

        #region system proxy settings
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption
          (IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        public const string ProxyIP = "127.0.0.1";
        public const short ProxyPort = 31080;
        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        };
        //set ALL_PROXY=socks5://127.0.0.1:31080
        public static bool SetSysProxy(bool on)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey
               ("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            bool settingsReturn, refreshReturn;
            if (on){ 
                registry.SetValue("ProxyEnable", 1);
                registry.SetValue
                ("ProxyServer", $"SOCKS5={ProxyIP}:{ProxyPort}");
                if ((int)registry.GetValue("ProxyEnable", 0) == 0)
                {
                    return false;
                }
            }
            else
            {
                registry.SetValue("ProxyEnable", 0);
                registry.SetValue("ProxyServer", 0);
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