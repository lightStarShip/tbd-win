using System;
using System.Windows.Forms;
using tbd.Controller;

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

        private void loadQRImage(object sender, EventArgs e)
        {
        }

        private void importThisAccount(object sender, EventArgs e)
        {
            string walletData = walletDataText.Text;
            Console.WriteLine(walletData.Length);
            if (walletData.Length < 20)
            {
                return;
            }

        }

        private void ImportImage_Click(object sender, EventArgs e)
        {

            string qrPath = pathofQR.Text;
        }
    }
}
