using NLog;
using tbd.Model;
using tbd.Properties;
using tbd.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tbd.Controller
{

    /// <summary>
    /// Processing the PAC file content
    /// </summary>
    public class PACDaemon
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static string S_PAC_FILE = null;
        public static string S_USER_RULE_FILE = null;
        public static string S_USER_ABP_FILE = null;
        private Configuration config;

        FileSystemWatcher PACFileWatcher;
        FileSystemWatcher UserRuleFileWatcher;

        public event EventHandler PACFileChanged;
        public event EventHandler UserRuleFileChanged;

        public PACDaemon(Configuration config)
        {
            this.config = config;
            TouchPACFile();
            TouchUserRuleFile();

            this.WatchPacFile();
            this.WatchUserRuleFile();
        }


        public string TouchPACFile()
        {
            if (!File.Exists(PacFilePath()))
            {
                GeositeUpdater.MergeAndWritePACFile(config.geositeDirectGroups, config.geositeProxiedGroups, config.geositePreferDirect);
            }
            return PacFilePath();
        }

        internal string TouchUserRuleFile()
        {
            if (!File.Exists(UserRulePath()))
            {
                File.WriteAllText(UserRulePath(), Resources.user_rule);
            }
            return UserRulePath();
        }

        internal string GetPACContent()
        {
            if (!File.Exists(PacFilePath()))
            {
                GeositeUpdater.MergeAndWritePACFile(config.geositeDirectGroups, config.geositeProxiedGroups, config.geositePreferDirect);
            }
            return File.ReadAllText(PacFilePath(), Encoding.UTF8);
        }


        private void WatchPacFile()
        {
            PACFileWatcher?.Dispose();
            PACFileWatcher = new FileSystemWatcher(Program.WorkingDirectory);
            PACFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            PACFileWatcher.Filter = PacFilePath();
            PACFileWatcher.Changed += PACFileWatcher_Changed;
            PACFileWatcher.Created += PACFileWatcher_Changed;
            PACFileWatcher.Deleted += PACFileWatcher_Changed;
            PACFileWatcher.Renamed += PACFileWatcher_Changed;
            PACFileWatcher.EnableRaisingEvents = true;
        }

        private void WatchUserRuleFile()
        {
            UserRuleFileWatcher?.Dispose();
            UserRuleFileWatcher = new FileSystemWatcher(Program.WorkingDirectory);
            UserRuleFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            UserRuleFileWatcher.Filter = UserRulePath();
            UserRuleFileWatcher.Changed += UserRuleFileWatcher_Changed;
            UserRuleFileWatcher.Created += UserRuleFileWatcher_Changed;
            UserRuleFileWatcher.Deleted += UserRuleFileWatcher_Changed;
            UserRuleFileWatcher.Renamed += UserRuleFileWatcher_Changed;
            UserRuleFileWatcher.EnableRaisingEvents = true;
        }

        public static string PacFilePath()
        {
            if (S_PAC_FILE == null)
            {
                S_PAC_FILE = Utils.GetAppDataPath("pac.txt");
            }
            return S_PAC_FILE;
        }
        public static string UserRulePath()
        {
            if (S_USER_RULE_FILE == null)
            {
                S_USER_RULE_FILE = Utils.GetAppDataPath("user-rule.txt");
            }
            return S_USER_RULE_FILE;
        }
        public static string UserABPPath()
        {
            if (S_USER_ABP_FILE == null)
            {
                S_USER_ABP_FILE = Utils.GetAppDataPath("abp.txt");
            }
            return S_USER_ABP_FILE;
        }
        
        
        #region FileSystemWatcher.OnChanged()
        // FileSystemWatcher Changed event is raised twice
        // http://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice
        // Add a short delay to avoid raise event twice in a short period
        private void PACFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (PACFileChanged != null)
            {
                logger.Info($"Detected: PAC file '{e.Name}' was {e.ChangeType.ToString().ToLower()}.");
                Task.Factory.StartNew(() =>
                {
                    ((FileSystemWatcher)sender).EnableRaisingEvents = false;
                    System.Threading.Thread.Sleep(10);
                    PACFileChanged(this, new EventArgs());
                    ((FileSystemWatcher)sender).EnableRaisingEvents = true;
                });
            }
        }

        private void UserRuleFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (UserRuleFileChanged != null)
            {
                logger.Info($"Detected: User Rule file '{e.Name}' was {e.ChangeType.ToString().ToLower()}.");
                Task.Factory.StartNew(() =>
                {
                    ((FileSystemWatcher)sender).EnableRaisingEvents = false;
                    System.Threading.Thread.Sleep(10);
                    UserRuleFileChanged(this, new EventArgs());
                    ((FileSystemWatcher)sender).EnableRaisingEvents = true;
                });
            }
        }
        #endregion
    }
}
