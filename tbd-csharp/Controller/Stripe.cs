using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tbd.Controller
{
    public class Stripe
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string Stripe_FILE = "_stripe.json";
        public string cus_id;
        public Int64 expire_day;
        public Int64 update_time;

        [JsonIgnore]
        string walletAddr;
        public static string GetStripeFileName(string wAddr)
        {
            return string.Format("{0}{1}", wAddr, Stripe_FILE);
        }
        public void SaveToDisk(string wAddr)
        {
            FileStream wFileStream = null;
            StreamWriter wStreamWriter = null;
            try
            {
                string file_name = GetStripeFileName(wAddr);
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
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });
            stripe.SaveToDisk(this.walletAddr);
            Console.WriteLine($"======>>>reload stripe basic: [{stripe.cus_id}]");
        }
        public static Stripe LoadStripe(string wAddr)
        {
            string file_name = GetStripeFileName(wAddr);
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
                ObjectCreationHandling = ObjectCreationHandling.Replace
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
    }
}
