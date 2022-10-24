using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using tbd.Controller;
using tbd.Util;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace tbd.View
{
    public partial class WalletImport : Form
    {
        private SimpleController controller;
        public WalletImport(SimpleController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        private void FindQRImgPath(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Image files (*.jpg, *.gif, *.bmp) | *.jpg; *.gif; *.bmp; | All Files (*.*) | *.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            {
                Bitmap target;
                using (Stream bmpStream = File.Open(openFileDialog.FileName,
                    FileMode.Open))
                {
                    Image image = Image.FromStream(bmpStream);

                    target = new Bitmap(image);
                }
                var source = new BitmapLuminanceSource(target);
                var bitmap = new BinaryBitmap(new HybridBinarizer(source));
                QRCodeReader reader = new QRCodeReader();
                var result = reader.decode(bitmap);
                if (result == null)
                {
                    MessageBox.Show("Invalid Wallet QR", "Tips");
                    return;
                }
                Console.WriteLine($"================>>>>{result.Text}");
                ImportAction(result.Text);
            }
        }

        private void ImportAction(string wData)
        {
            string passwrod = "";
            DialogResult result = ViewUtils.ShowInputDialogBox(ref passwrod, "Input The Password Of This Wallet", "Authorization", 300, 200);
            if (result != DialogResult.OK)
            {
                return;
            }
            if (false == SimpleDelegate.LoadWalletWin(wData, passwrod))
            {
                MessageBox.Show(I18N.GetString("Load Wallet Failed"), I18N.GetString("Tips"));
                return;
            }
            SimpleDelegate.wallet.SaveToDisk(wData, passwrod);
            MessageBox.Show(I18N.GetString("Load Wallet Success"), I18N.GetString("Tips"));
            this.Close();
        }

        private void ImportByJsonData(object sender, EventArgs e)
        {
            string walletData = walletDataText.Text;
            if (walletData.Length < 20)
            {
                MessageBox.Show(I18N.GetString("Invalid Wallet Json Data"), I18N.GetString("Tips"));
                return;
            }
            ImportAction(walletData);
        }
    }
}
