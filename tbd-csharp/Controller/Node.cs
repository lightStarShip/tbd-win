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
    public class Node
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static readonly string NODE_FILE = "nodes.json";
        public static event EventHandler NodeChanged;

        public static Dictionary<string, Node> NodeCache = new Dictionary<string, Node>();

        [JsonProperty("location")]
        public string LocalName;
        [JsonProperty("icon")]
        public string ICOName;
        [JsonProperty("ip")]
        public string Host;
        [JsonProperty("address")]
        public string NodeAddr;

        [JsonProperty("free")]
        public bool IsFree;

        [JsonIgnore]
        public float PingVal = 0.0f;

        public static void LoadNodeList(bool fromSrv = false)
        {
            string content = "";
            bool need_save = false;
            if (false == File.Exists(NODE_FILE) || fromSrv == true)
            {
                IntPtr nPtr = SimpleDelegate.NodeConfigData();
                content = Marshal.PtrToStringAnsi(nPtr);
                if (content == null)
                {
                    Console.WriteLine("======>>> failed to load node config");
                    return;
                }
                need_save = true;
            }
            else
            {
                content = File.ReadAllText(NODE_FILE);
            }
            List<Node> nodeList = JsonConvert.DeserializeObject<List<Node>>(content);
            if (need_save)
            {
                SaveToDisk(content);
            }
            else
            {
                Thread t = new Thread(new ThreadStart(ReloadNodeThread));
                t.IsBackground = true;
                t.Start();
            }

            FillCache(nodeList);
            if (fromSrv)
            {
                NodeChanged?.Invoke(null, new EventArgs());
            }
        }
        private static void FillCache(List<Node> nodeList)
        {
            NodeCache.Clear();
            bool isVip = SimpleDelegate.stripe.IsVip();
            Node first = null;
            foreach (Node n in nodeList)
            {
                if (n.IsFree == false && isVip == false)
                {
                    continue;
                }
                NodeCache[n.NodeAddr] = n;
                if (first == null)
                {
                    first = n;
                }
            }
            if (first == null || SimpleDelegate.stripe.currentNode != null)
            {
                return;
            }

            SimpleDelegate.stripe.SetCurrentNode(first.NodeAddr);
        } 
        private static void SaveToDisk(string content)
        {
            FileStream wFileStream = null;
            StreamWriter wStreamWriter = null;
            try
            { 
                wFileStream = File.Open(NODE_FILE, FileMode.Create);
                wStreamWriter = new StreamWriter(wFileStream);
                wStreamWriter.Write(content);
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
        private static void ReloadNodeThread()
        {
            IntPtr nPtr = SimpleDelegate.NodeConfigData();
            string content = Marshal.PtrToStringAnsi(nPtr);
            if (content == null)
            {
                Console.WriteLine("======>>> reload node config thread failed, no content");
                return;
            }
            List<Node> nodeList = JsonConvert.DeserializeObject<List<Node>>(content, new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                NullValueHandling = NullValueHandling.Ignore
            });
            
            FillCache(nodeList);
            SaveToDisk(content);
            NodeChanged?.Invoke(null, new EventArgs());
        }

        public string GetPingStr()
        {
            if (this.PingVal <= 0)
            {
                return "\t-1.00 ms";
            }
            return $"\t{this.PingVal.ToString("0.00")} ms";
        }

        public static void MultiPing()
        {
            foreach (KeyValuePair<string, Node> nodeItem in Node.NodeCache)
            {
                Node n = nodeItem.Value;
                n.PingVal = SimpleDelegate.PingValWin(n.NodeAddr, n.Host);
            }

            NodeChanged?.Invoke(null, new EventArgs());
        }
    }
}
