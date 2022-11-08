using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tbd.Controller;
using tbd.Util;

namespace tbd.View
{
    public partial class AccountDetails : Form
    {
        public AccountDetails()
        {
            InitializeComponent();
            string cid = SimpleDelegate.stripe.cus_id;
            if (cid == null)
            {
                cid = "";
            }
            this.accountID.Text = cid;
            this.bAddress.Text = SimpleDelegate.wallet.Address;
            float balance = SimpleDelegate.BalanceWin(0);
            this.balanceLbl.Text = balance.ToString("0.00");
            this.accQRImg.Image = Utils.ToImage(cid);
        }
    }
}
