using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using tbd.Controller;
using tbd.Localization;
using tbd.Model;
using tbd.Properties;
using tbd.Util;
using tbd.Views;

namespace tbd.View
{
    public class MenuViewController
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private SimpleController controller;
        public UpdateChecker updateChecker;

        private NotifyIcon _notifyIcon;
        private Icon icon, icon_in, icon_out, icon_both, previousIcon;

        private bool _isStartupCheck;
        private string _urlToOpen;

        private ContextMenu contextMenu1;
        private MenuItem SeperatorItem;
        private MenuItem ServersItem;
        private MenuItem startStopItem;
        private MenuItem curNodeItem;

        private ConfigForm configForm;
        private WalletImport walletImportForm;
        private AccountDetails accountDetalsForm;
        private LogForm logForm;

        private System.Windows.Window serverSharingWindow;
        private System.Windows.Window hotkeysWindow;
        private System.Windows.Window forwardProxyWindow;
        private System.Windows.Window onlineConfigWindow;

        // color definition for icon color transformation
        private readonly Color colorMaskOn = Color.FromArgb(255, 255, 213, 34);
        private readonly Color colorMaskDarkSilver = Color.FromArgb(128, 192, 192, 192);
        private readonly Color colorMaskLightSilver = Color.FromArgb(192, 192, 192);
        private readonly Color colorMaskEclipse = Color.FromArgb(192, 64, 64, 64);//255,193,193,188

        public MenuViewController(SimpleController controller)
        {
            this.controller = controller;

            LoadMenu();

            controller.PACFileReadyToOpen += controller_FileReadyToOpen;
            controller.UserRuleFileReadyToOpen += controller_FileReadyToOpen;
            controller.Errored += controller_Errored;
            controller.UpdatePACFromGeositeCompleted += controller_UpdatePACFromGeositeCompleted;
            controller.UpdatePACFromGeositeError += controller_UpdatePACFromGeositeError;

            _notifyIcon = new NotifyIcon();
            UpdateTrayIconAndNotifyTextForSimple();
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenu = contextMenu1;
            _notifyIcon.BalloonTipClicked += notifyIcon1_BalloonTipClicked;
            _notifyIcon.MouseClick += notifyIcon1_Click;
            _notifyIcon.MouseDoubleClick += notifyIcon1_DoubleClick;
            _notifyIcon.BalloonTipClosed += _notifyIcon_BalloonTipClosed;
            controller.TrafficChanged += controller_TrafficChanged;

            updateChecker = new UpdateChecker();
            updateChecker.CheckUpdateCompleted += updateChecker_CheckUpdateCompleted;

            UpdateServersMenu();
            Node.NodeChanged += menu_NodeChanged;

            Configuration config = controller.GetCurrentConfiguration();

            if (!SimpleDelegate.HasWallet())
            {
                ShowImportWalletForm();
            }
        }

        #region Tray Icon

        private void UpdateTrayIconAndNotifyTextForSimple()
        {
            bool isOn = SimpleDelegate.IsProxySet();
            this.startStopItem.Text = isOn ? I18N.GetString("Stop") : I18N.GetString("Start");

            Color colorMask = SetStatusColorMask(isOn);
            Size iconSize = SelectIconSize();

            UpdateIconSet(colorMask, iconSize, out icon, out icon_in, out icon_out, out icon_both);

            previousIcon = icon;
            _notifyIcon.Icon = previousIcon;
          
            // show more info by hacking the P/Invoke declaration for NOTIFYICONDATA inside Windows Forms
            string text = I18N.GetString("TheBigDipper") + " " + UpdateChecker.Version + "\n" +
                          (isOn ? I18N.GetString("System Proxy On: " + $"{SimpleDelegate.ProxyIP}:{SimpleDelegate.ProxyPort}") :
                          I18N.GetString("System Proxy Off"));
            if (text.Length > 127)
            {
                text = text.Substring(0, 126 - 3) + "...";
            }
            ViewUtils.SetNotifyIconText(_notifyIcon, text);
        }

        /*private void UpdateTrayIconAndNotifyText()
        {
            Configuration config = controller.GetCurrentConfiguration();
            bool enabled = config.enabled;
            bool global = config.global;

            Color colorMask = SelectColorMask(enabled, global);
            Size iconSize = SelectIconSize();

            UpdateIconSet(colorMask, iconSize, out icon, out icon_in, out icon_out, out icon_both);

            previousIcon = icon;
            _notifyIcon.Icon = previousIcon;

            string serverInfo = null;
            if (controller.GetCurrentStrategy() != null)
            {
                serverInfo = controller.GetCurrentStrategy().Name;
            }
            else
            {
                serverInfo = config.GetCurrentServer().ToString();
            }
            // show more info by hacking the P/Invoke declaration for NOTIFYICONDATA inside Windows Forms
            string text = I18N.GetString("TheBigDipper") + " " + UpdateChecker.Version + "\n" +
                          (enabled ?
                              I18N.GetString("System Proxy On: ") + (global ? I18N.GetString("Global") : I18N.GetString("PAC")) :
                              I18N.GetString("Running: Port {0}", config.localPort))  // this feedback is very important because they need to know TheBigDipper is running
                          + "\n" + serverInfo;
            if (text.Length > 127)
            {
                text = text.Substring(0, 126 - 3) + "...";
            }
            ViewUtils.SetNotifyIconText(_notifyIcon, text);
        }*/

        /// <summary>
        /// Determine the icon size based on the screen DPI.
        /// </summary>
        /// <returns></returns>
        /// https://stackoverflow.com/a/40851713/2075611
        private Size SelectIconSize()
        {
            Size size = new Size(32, 32);
            int dpi = ViewUtils.GetScreenDpi();
            if (dpi < 97)
            {
                // dpi = 96;
                size = new Size(16, 16);
            }
            else if (dpi < 121)
            {
                // dpi = 120;
                size = new Size(20, 20);
            }
            else if (dpi < 145)
            {
                // dpi = 144;
                size = new Size(24, 24);
            }
            else
            {
                // dpi = 168;
                size = new Size(28, 28);
            }
            return size;
        }
        private Color SetStatusColorMask(bool curStatus)
        {
            if (curStatus)
            {
                return colorMaskOn;
            }

            Utils.WindowsThemeMode currentWindowsThemeMode = Utils.GetWindows10SystemThemeSetting();
            if (currentWindowsThemeMode == Utils.WindowsThemeMode.Light)
            {
                return colorMaskDarkSilver;
            }

            return colorMaskLightSilver;
        }
        private Color SelectColorMask(bool isProxyEnabled, bool isGlobalProxy)
        {
            Color colorMask = Color.White;

            Utils.WindowsThemeMode currentWindowsThemeMode = Utils.GetWindows10SystemThemeSetting();

            if (isProxyEnabled)
            {
                if (isGlobalProxy)  // global
                {
                    colorMask = colorMaskOn;
                }
                else  // PAC
                {
                    if (currentWindowsThemeMode == Utils.WindowsThemeMode.Light)
                    {
                        colorMask = colorMaskEclipse;
                    }
                }
            }
            else  // disabled
            {
                if (currentWindowsThemeMode == Utils.WindowsThemeMode.Light)
                {
                    colorMask = colorMaskDarkSilver;
                }
                else
                {
                    colorMask = colorMaskLightSilver;
                }
            }

            return colorMask;
        }

        private void UpdateIconSet(Color colorMask, Size size,
            out Icon icon, out Icon icon_in, out Icon icon_out, out Icon icon_both)
        {
            Bitmap iconBitmap;

            // generate the base icon
            iconBitmap = ViewUtils.ChangeBitmapColor(Resources.ss32Fill, colorMask);
            iconBitmap = ViewUtils.AddBitmapOverlay(iconBitmap, Resources.ss32Outline);

            icon = Icon.FromHandle(ViewUtils.ResizeBitmap(iconBitmap, size.Width, size.Height).GetHicon());
            icon_in = Icon.FromHandle(ViewUtils.ResizeBitmap(ViewUtils.AddBitmapOverlay(iconBitmap, Resources.ss32In), size.Width, size.Height).GetHicon());
            icon_out = Icon.FromHandle(ViewUtils.ResizeBitmap(ViewUtils.AddBitmapOverlay(iconBitmap, Resources.ss32In), size.Width, size.Height).GetHicon());
            icon_both = Icon.FromHandle(ViewUtils.ResizeBitmap(ViewUtils.AddBitmapOverlay(iconBitmap, Resources.ss32In, Resources.ss32Out), size.Width, size.Height).GetHicon());
        }

        #endregion

        #region MenuItems and MenuGroups
        private void menu_NodeChanged(object sender, EventArgs e)
        {
            UpdateServersMenu();
        }
        private MenuItem CreateMenuItem(string text, EventHandler click)
        {
            return new MenuItem(I18N.GetString(text), click);
        }

        private MenuItem CreateMenuGroup(string text, MenuItem[] items)
        {
            return new MenuItem(I18N.GetString(text), items);
        }

        private void LoadMenu()
        {
            bool curStatus = SimpleDelegate.IsProxySet();
            if (curStatus)
            {
                SimpleDelegate.SetSysProxy(false);
            }
            this.contextMenu1 = new ContextMenu(new MenuItem[] {

                this.startStopItem = CreateMenuItem("Start", new EventHandler(this.Start_Stop)),
               
                this.ServersItem = CreateMenuGroup("Servers", new MenuItem[] {
                        this.SeperatorItem = new MenuItem("-"),
                        CreateMenuItem("Ping Nodes", new EventHandler(this.Ping_clicked)),
                        CreateMenuItem("Reload Nodes", new EventHandler(this.Reload_clicked)),
                }),
                CreateMenuItem("Account", new EventHandler(this.ShowAccountDetails)),
                CreateMenuItem("Command Line", new EventHandler(this.CopyCmdLine)),
                new MenuItem("-"),
                /*this.AutoStartupItem = CreateMenuItem("Start on Boot", new EventHandler(this.AutoStartupItem_Click)),
                this.ProtocolHandlerItem = CreateMenuItem("Associate ss:// Links", new EventHandler(this.ProtocolHandlerItem_Click)),
                this.ShareOverLANItem = CreateMenuItem("Allow other Devices to connect", new EventHandler(this.ShareOverLANItem_Click)),
                new MenuItem("-"),
                this.hotKeyItem = CreateMenuItem("Edit Hotkeys...", new EventHandler(this.hotKeyItem_Click)),
                CreateMenuGroup("Help", new MenuItem[] {
                    CreateMenuItem("Show Logs...", new EventHandler(this.ShowLogItem_Click)),
                    this.VerboseLoggingToggleItem = CreateMenuItem( "Verbose Logging", new EventHandler(this.VerboseLoggingToggleItem_Click) ),
                    this.ShowPluginOutputToggleItem = CreateMenuItem("Show Plugin Output", new EventHandler(this.ShowPluginOutputToggleItem_Click)),
                    this.WriteI18NFileItem = CreateMenuItem("Write translation template",new EventHandler(WriteI18NFileItem_Click)),
                    CreateMenuGroup("Updates...", new MenuItem[] {
                        CreateMenuItem("Check for Updates...", new EventHandler(this.checkUpdatesItem_Click)),
                        new MenuItem("-"),
                        this.autoCheckUpdatesToggleItem = CreateMenuItem("Check for Updates at Startup", new EventHandler(this.autoCheckUpdatesToggleItem_Click)),
                        this.checkPreReleaseToggleItem = CreateMenuItem("Check Pre-release Version", new EventHandler(this.checkPreReleaseToggleItem_Click)),
                    }),
                    CreateMenuItem("About...", new EventHandler(this.AboutItem_Click)),
                }),*/

                CreateMenuItem("Updates...", new EventHandler(this.checkUpdatesItem_Click)),
                CreateMenuItem("About...", new EventHandler(this.AboutItem_Click)),
                new MenuItem("-"),
                CreateMenuItem("Quit", new EventHandler(this.Quit_Click))
            });
        }

        #endregion

        private void controller_TrafficChanged(object sender, EventArgs e)
        {
            if (icon == null)
                return;

            Icon newIcon;

            bool hasInbound = controller.trafficPerSecondQueue.Last().inboundIncreasement > 0;
            bool hasOutbound = controller.trafficPerSecondQueue.Last().outboundIncreasement > 0;

            if (hasInbound && hasOutbound)
                newIcon = icon_both;
            else if (hasInbound)
                newIcon = icon_in;
            else if (hasOutbound)
                newIcon = icon_out;
            else
                newIcon = icon;

            if (newIcon != this.previousIcon)
            {
                this.previousIcon = newIcon;
                _notifyIcon.Icon = newIcon;
            }
        }

        void controller_Errored(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.GetException().ToString(), I18N.GetString("TheBigDipper Error: {0}", e.GetException().Message));
        }

        private void LoadCurrentConfiguration()
        {
            Configuration config = controller.GetCurrentConfiguration();
        }

        #region Forms

        private void ShowConfigForm()
        {
            if (configForm != null)
            {
                configForm.Activate();
            }
            else
            {
                configForm = new ConfigForm(controller);
                configForm.Show();
                configForm.Activate();
                configForm.FormClosed += configForm_FormClosed;
            }
        }

        private void ShowImportWalletForm()
        {
            if (walletImportForm != null)
            {
                walletImportForm.Activate();
            }
            else
            {
                walletImportForm = new WalletImport();
                walletImportForm.Show();
                walletImportForm.Activate();
                walletImportForm.FormClosed += importForm_FormClosed;
            }
        }

        private void ShowLogForm()
        {
            if (logForm != null)
            {
                logForm.Activate();
            }
            else
            {
                logForm = new LogForm(controller);
                logForm.Show();
                logForm.Activate();
                logForm.FormClosed += logForm_FormClosed;
            }
        }

        void logForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            logForm.Dispose();
            logForm = null;
        }

        void importForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            walletImportForm.Dispose();
            walletImportForm = null;
        }

        void configForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            configForm.Dispose();
            configForm = null;
            var config = controller.GetCurrentConfiguration();
            if (config.firstRun)
            {
                CheckUpdateForFirstRun();
                ShowBalloonTip(
                    I18N.GetString("TheBigDipper is here"),
                    I18N.GetString("You can turn on/off TheBigDipper in the context menu"),
                    ToolTipIcon.Info,
                    0
                );
                config.firstRun = false;
            }
        }

        #endregion

        #region Misc

        void ShowBalloonTip(string title, string content, ToolTipIcon icon, int timeout)
        {
            _notifyIcon.BalloonTipTitle = title;
            _notifyIcon.BalloonTipText = content;
            _notifyIcon.BalloonTipIcon = icon;
            _notifyIcon.ShowBalloonTip(timeout);
        }

        void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
        }

        private void _notifyIcon_BalloonTipClosed(object sender, EventArgs e)
        {
        }

        //TODO::
        private void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            /*UpdateTrayIconAndNotifyText();
            if (e.Button == MouseButtons.Middle)
            {
                ShowLogForm();
            }*/
        }

        private void notifyIcon1_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //ShowConfigForm();
                showAccountForm();
            }
        }

        private void CheckUpdateForFirstRun()
        {
            Configuration config = controller.GetCurrentConfiguration();
            if (config.firstRun)
                return;
            _isStartupCheck = true;
            Dispatcher.CurrentDispatcher.Invoke(() => updateChecker.CheckForVersionUpdate(3000));
        }

        public void ShowLogForm_HotKey()
        {
            ShowLogForm();
        }

        #endregion

        #region Main menu

        private void proxyItem_Click(object sender, EventArgs e)
        {
            if (forwardProxyWindow == null)
            {
                forwardProxyWindow = new System.Windows.Window()
                {
                    Title = LocalizationProvider.GetLocalizedValue<string>("ForwardProxy"),
                    Height = 400,
                    Width = 280,
                    MinHeight = 400,
                    MinWidth = 280,
                    Content = new ForwardProxyView()
                };
                forwardProxyWindow.Closed += ForwardProxyWindow_Closed;
                ElementHost.EnableModelessKeyboardInterop(forwardProxyWindow);
                forwardProxyWindow.Show();
            }
            forwardProxyWindow.Activate();
        }

        private void ForwardProxyWindow_Closed(object sender, EventArgs e)
        {
            forwardProxyWindow = null;
        }

        public void CloseForwardProxyWindow() => forwardProxyWindow.Close();

        private void OnlineConfig_Click(object sender, EventArgs e)
        {
            if (onlineConfigWindow == null)
            {
                onlineConfigWindow = new System.Windows.Window()
                {
                    Title = LocalizationProvider.GetLocalizedValue<string>("OnlineConfigDelivery"),
                    Height = 510,
                    Width = 480,
                    MinHeight = 510,
                    MinWidth = 480,
                    Content = new OnlineConfigView()
                };
                onlineConfigWindow.Closed += OnlineConfigWindow_Closed;
                ElementHost.EnableModelessKeyboardInterop(onlineConfigWindow);
                onlineConfigWindow.Show();
            }
            onlineConfigWindow.Activate();
        }

        private void OnlineConfigWindow_Closed(object sender, EventArgs e)
        {
            onlineConfigWindow = null;
        }

        private void hotKeyItem_Click(object sender, EventArgs e)
        {
            if (hotkeysWindow == null)
            {
                hotkeysWindow = new System.Windows.Window()
                {
                    Title = LocalizationProvider.GetLocalizedValue<string>("Hotkeys"),
                    Height = 260,
                    Width = 320,
                    MinHeight = 260,
                    MinWidth = 320,
                    Content = new HotkeysView()
                };
                hotkeysWindow.Closed += HotkeysWindow_Closed;
                ElementHost.EnableModelessKeyboardInterop(hotkeysWindow);
                hotkeysWindow.Show();
            }
            hotkeysWindow.Activate();
        }

        private void HotkeysWindow_Closed(object sender, EventArgs e)
        {
            hotkeysWindow = null;
        }

        public void CloseHotkeysWindow() => hotkeysWindow.Close();

        private void Start_Stop(object sender, EventArgs e)
        {
            string curNode = SimpleDelegate.stripe.currentNode;
            if (curNode == null)
            {
                MessageBox.Show(I18N.GetString("Load Server First"), I18N.GetString("Error"));
                return;
            }
            bool curStatus = SimpleDelegate.IsProxySet();
            
            bool success = SimpleDelegate.SetSysProxy(!curStatus);

            if (!success){
                MessageBox.Show("Set System Proxy failed", "Error");
                return;
            }

            if (curStatus)
            {
                SimpleDelegate.StopProxy();
            } else { 
                success = SimpleDelegate.StartSimpleProtocol();
                if (!success)
                {
                    MessageBox.Show("Start Local Proxy failed", "Error");
                    return;
                }
            }
            UpdateTrayIconAndNotifyTextForSimple();
        }
        private void showAccountForm()
        {
            if (accountDetalsForm != null)
            {
                accountDetalsForm.Activate();
            }
            else
            {
                accountDetalsForm = new AccountDetails();
                accountDetalsForm.Show();
                accountDetalsForm.Activate();
                accountDetalsForm.FormClosed += accountFormClose;
            }
        }
        private void ShowAccountDetails(object sender, EventArgs e)
        {
            showAccountForm();
        }

        void accountFormClose(object sender, FormClosedEventArgs e)
        {
            accountDetalsForm.Dispose();
            accountDetalsForm = null;
        }
        private void CopyCmdLine(object sender, EventArgs e)
        {
            Clipboard.SetText(SimpleDelegate.CmdLine);
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            SimpleDelegate.StopProxy();
            SimpleDelegate.SetSysProxy(false);
            controller.Stop();
            _notifyIcon.Visible = false;
            Application.Exit();
        }

        #endregion

        #region Server

        private void Ping_clicked(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(Node.MultiPing));
            t.IsBackground = true;
            t.Start();
        }
        private void Reload_clicked (object sender, EventArgs e)
        {
            Node.LoadNodeList(true);
        }

        private void UpdateServersMenu()
        {
            var items = ServersItem.MenuItems;
            while (items[0] != SeperatorItem)
            {
                items.RemoveAt(0);
            }
            int serverCount = 0;
            foreach (KeyValuePair<string, Node> nodeItem in Node.NodeCache)
            {
                string addr = nodeItem.Key;
                Node node = nodeItem.Value;
                string itemLabel = string.Format("{0}{1}", node.LocalName, node.GetPingStr());
                MenuItem item = new MenuItem(itemLabel);
                item.Tag = node.NodeAddr;
                item.Click += NodeChanged_Click;
                items.Add(serverCount, item);
                serverCount++;
                string curNode = SimpleDelegate.stripe.currentNode;
                if (curNode == node.NodeAddr)
                {
                    this.curNodeItem = item;
                    item.Checked = true;
                }
            }
            // user wants a seperator item between strategy and servers menugroup
            items.Add(serverCount++, new MenuItem("-"));
        }

        private void NodeChanged_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string addr = (string)item.Tag;
            SimpleDelegate.stripe.SetCurrentNode(addr);
            item.Checked = true;
            if (this.curNodeItem != null)
            {
                this.curNodeItem.Checked = false;
            }
            this.curNodeItem = item;

            Node n = Node.NodeCache[addr];
            if (n == null)
            {
                Console.WriteLine($"======>>> invalid node addr: {addr}");
                return;
            }

            IntPtr errStr = SimpleDelegate.ChangeSrvWin(n.NodeAddr, n.Host);
            string err = Marshal.PtrToStringAnsi(errStr);
            if (err != null)
            {
                Console.WriteLine($"======>>> change node failed: { err}");
                return;
            }
        }

        private void AServerItem_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            controller.SelectServerIndex((int)item.Tag);
        }

        private void AStrategyItem_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            controller.SelectStrategy((string)item.Tag);
        }

        private void Config_Click(object sender, EventArgs e)
        {
            ShowConfigForm();
        }

        void openURLFromQRCode()
        {
            Process.Start(_urlToOpen);
        }

        private void QRCodeItem_Click(object sender, EventArgs e)
        {
            if (serverSharingWindow == null)
            {
                serverSharingWindow = new System.Windows.Window()
                {
                    Title = LocalizationProvider.GetLocalizedValue<string>("ServerSharing"),
                    Height = 400,
                    Width = 660,
                    MinHeight = 400,
                    MinWidth = 660,
                    Content = new ServerSharingView()
                };
                serverSharingWindow.Closed += ServerSharingWindow_Closed;
                ElementHost.EnableModelessKeyboardInterop(serverSharingWindow);
                serverSharingWindow.Show();
            }
            serverSharingWindow.Activate();
        }

        private void ServerSharingWindow_Closed(object sender, EventArgs e)
        {
            serverSharingWindow = null;
        }

        private void ScanQRCodeItem_Click(object sender, EventArgs e)
        {
            var result = Utils.ScanQRCodeFromScreen();
            if (result != null)
            {
                if (result.ToLowerInvariant().StartsWith("http://") || result.ToLowerInvariant().StartsWith("https://"))
                {
                    _urlToOpen = result;
                    openURLFromQRCode();
                }
                else if (controller.AddServerBySSURL(result))
                {
                    ShowConfigForm();
                }
                else
                {
                    MessageBox.Show(I18N.GetString("Invalid QR Code content: {0}", result));
                }
                return;
            }
            else
                MessageBox.Show(I18N.GetString("No QRCode found. Try to zoom in or move it to the center of the screen."));
        }

        private void ImportURLItem_Click(object sender, EventArgs e)
        {
            if (controller.AskAddServerBySSURL(Clipboard.GetText(TextDataFormat.Text)))
            {
                ShowConfigForm();
            }
        }

        #endregion

        #region PAC

        private void UpdateOnlinePACURLItem_Click(object sender, EventArgs e)
        {
            string origPacUrl = controller.GetCurrentConfiguration().pacUrl;
            string pacUrl = Microsoft.VisualBasic.Interaction.InputBox(
                I18N.GetString("Please input PAC Url"),
                I18N.GetString("Edit Online PAC URL"),
                origPacUrl, -1, -1);
            if (!string.IsNullOrEmpty(pacUrl) && pacUrl != origPacUrl)
            {
                controller.SavePACUrl(pacUrl);
            }
        }

        private void SecureLocalPacUrlToggleItem_Click(object sender, EventArgs e)
        {
            Configuration configuration = controller.GetCurrentConfiguration();
            controller.ToggleSecureLocalPac(!configuration.secureLocalPac);
        }

        private void RegenerateLocalPacOnUpdateItem_Click(object sender, EventArgs e)
        {
            var config = controller.GetCurrentConfiguration();
            controller.ToggleRegeneratePacOnUpdate(!config.regeneratePacOnUpdate);
        }

        private void CopyLocalPacUrlItem_Click(object sender, EventArgs e)
        {
            controller.CopyPacUrl();
        }


        private void EditPACFileItem_Click(object sender, EventArgs e)
        {
            controller.TouchPACFile();
        }

        private async void UpdatePACFromGeositeItem_Click(object sender, EventArgs e)
        {
            await GeositeUpdater.UpdatePACFromGeosite();
        }

        private void EditUserRuleFileForGeositeItem_Click(object sender, EventArgs e)
        {
            controller.TouchUserRuleFile();
        }

        void controller_FileReadyToOpen(object sender, SimpleController.PathEventArgs e)
        {
            string argument = @"/select, " + e.Path;

            Process.Start("explorer.exe", argument);
        }

        void controller_UpdatePACFromGeositeError(object sender, System.IO.ErrorEventArgs e)
        {
            ShowBalloonTip(I18N.GetString("Failed to update PAC file"), e.GetException().Message, ToolTipIcon.Error, 5000);
            logger.LogUsefulException(e.GetException());
        }

        void controller_UpdatePACFromGeositeCompleted(object sender, GeositeResultEventArgs e)
        {
            string result = e.Success
                ? I18N.GetString("PAC updated")
                : I18N.GetString("No updates found. Please report to Geosite if you have problems with it.");
            ShowBalloonTip(I18N.GetString("TheBigDipper"), result, ToolTipIcon.Info, 1000);
        }

        #endregion

        #region Help


        private void ShowLogItem_Click(object sender, EventArgs e)
        {
            ShowLogForm();
        }

   
        private void WriteI18NFileItem_Click(object sender, EventArgs e)
        {
            File.WriteAllText(I18N.I18N_FILE, Resources.i18n_csv, Encoding.UTF8);
        }

        #endregion

        #region Update

        void updateChecker_CheckUpdateCompleted(object sender, EventArgs e)
        {
            if (!_isStartupCheck && updateChecker.NewReleaseZipFilename == null)
            {
                ShowBalloonTip(I18N.GetString("TheBigDipper"), I18N.GetString("No update is available"), ToolTipIcon.Info, 5000);
            }
            _isStartupCheck = false;
        }

        private async void checkUpdatesItem_Click(object sender, EventArgs e)
        {
            await updateChecker.CheckForVersionUpdate();
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/lightStarShip/tbd-win");
        }

        #endregion
    }
}
