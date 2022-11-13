using System;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using NLog;
using tbd.Util;

namespace tbd.Controller
{
    public class Wallet
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static readonly string S_WALLET_FILE = "wallet.json";
        [JsonIgnore]
        public string Address;
        public string RawData;
        public string Pwd;

        static string WalletFilePath()
        {
            return Utils.GetAppDataPath(S_WALLET_FILE);
        }
        public static void CreateWallet(string raw, string pwd)
        {
            Wallet wallet = new Wallet();
            wallet.RawData = raw;
            wallet.Pwd = pwd;
            FileStream wFileStream = null;
            StreamWriter wStreamWriter = null;
            try
            {
                wFileStream = File.Open(WalletFilePath(), FileMode.Create);
                wStreamWriter = new StreamWriter(wFileStream);
                var jsonString = JsonConvert.SerializeObject(wallet, Formatting.Indented);
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

        public static Wallet LoadWallet()
        {
            if (false == File.Exists(Wallet.WalletFilePath()))
            {
                return new Wallet();
            }

            Wallet wallet = new Wallet();
            string content = File.ReadAllText(Wallet.WalletFilePath());
            wallet = JsonConvert.DeserializeObject<Wallet>(content, new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });
            if (true == SimpleDelegate.OpenWalletWin(wallet.RawData, wallet.Pwd))
            {
                IntPtr wPtr = SimpleDelegate.LibWalletAddress(); ;
                wallet.Address = Marshal.PtrToStringAnsi(wPtr);
                wallet.Pwd = "";
                Console.WriteLine($"================>>>{wallet.Address}");
                return wallet;
            }
            return new Wallet();
        }
    }
}