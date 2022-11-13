using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using tbd.Util;

namespace tbd.Controller
{
    public class Stripe
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string Stripe_FILE = "_stripe.json";
        public string cus_id;
        public string currentNode;
        public Int64 expire_day;
        public Int64 update_time;

        [JsonIgnore]
        string walletAddr;
        public static string GetStripeFilePath(string wAddr)
        {
            string file_name = string.Format("{0}{1}", wAddr, Stripe_FILE);
            return Utils.GetAppDataPath(file_name);
        }
        public void SaveToDisk(string wAddr)
        {
            FileStream wFileStream = null;
            StreamWriter wStreamWriter = null;
            try
            {
                string file_name = GetStripeFilePath(wAddr);
                wFileStream = File.Open(file_name, FileMode.Create);
                wStreamWriter = new StreamWriter(wFileStream);
                var jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
                wStreamWriter.Write(jsonString);
                wStreamWriter.Flush();
            }catch (Exception e)
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
        private void ReloadStripeBasic()
        {
            string cid = this.cus_id;
            if (cid == null)
            {
                cid = "";
            }
            IntPtr sPtr = SimpleDelegate.StripeBasic(this.walletAddr, "");
            string content = Marshal.PtrToStringAnsi(sPtr);
            Stripe stripe = JsonConvert.DeserializeObject<Stripe>(content, new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                NullValueHandling = NullValueHandling.Ignore
            });
            stripe.currentNode = this.currentNode;
            stripe.SaveToDisk(this.walletAddr);
            Console.WriteLine($"======>>>reload stripe basic: [{stripe.cus_id}]");
        }
        public static Stripe LoadStripe(string wAddr)
        {
            string file_name = GetStripeFilePath(wAddr);
            string content = "";
            bool need_save = false;
            if (false == File.Exists(file_name))
            {
                IntPtr sPtr = SimpleDelegate.StripeBasic(wAddr, "");
                content = Marshal.PtrToStringAnsi(sPtr);
                need_save = true;
            }
            else
            {
                content = File.ReadAllText(file_name);
            }

            Stripe stripe = JsonConvert.DeserializeObject<Stripe>(content, new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                NullValueHandling = NullValueHandling.Ignore
            });
            stripe.walletAddr = wAddr;

            if (need_save){
                stripe.SaveToDisk(wAddr);
            }
            else { 
                Thread t = new Thread(new ThreadStart(stripe.ReloadStripeBasic));
                t.IsBackground = true;
                t.Start();
            }
            return stripe;
        }

        public bool IsVip()
        {
            Int64 now = DateTimeOffset.Now.ToUnixTimeSeconds();
            Console.WriteLine($"======>>> expire day:{expire_day}  now:{now}");
            return expire_day > now + 5;
        }

        public void SetCurrentNode(string addr)
        {
            this.currentNode = addr;
            SaveToDisk(this.walletAddr);
        }
    }
}
