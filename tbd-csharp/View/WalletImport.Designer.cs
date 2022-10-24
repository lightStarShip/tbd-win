
namespace tbd.View
{
    partial class WalletImport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.loadByQR = new System.Windows.Forms.Button();
            this.importWallet = new System.Windows.Forms.Button();
            this.walletDataText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // loadByQR
            // 
            this.loadByQR.Location = new System.Drawing.Point(122, 310);
            this.loadByQR.Name = "loadByQR";
            this.loadByQR.Size = new System.Drawing.Size(214, 23);
            this.loadByQR.TabIndex = 3;
            this.loadByQR.Text = "Import By QR Code";
            this.loadByQR.UseVisualStyleBackColor = true;
            this.loadByQR.Click += new System.EventHandler(this.FindQRImgPath);
            // 
            // importWallet
            // 
            this.importWallet.Location = new System.Drawing.Point(175, 253);
            this.importWallet.Name = "importWallet";
            this.importWallet.Size = new System.Drawing.Size(132, 23);
            this.importWallet.TabIndex = 4;
            this.importWallet.Text = "import By Data ";
            this.importWallet.UseVisualStyleBackColor = true;
            this.importWallet.Click += new System.EventHandler(this.ImportByJsonData);
            // 
            // walletDataText
            // 
            this.walletDataText.Location = new System.Drawing.Point(12, 37);
            this.walletDataText.Multiline = true;
            this.walletDataText.Name = "walletDataText";
            this.walletDataText.Size = new System.Drawing.Size(465, 210);
            this.walletDataText.TabIndex = 5;
            // 
            // WalletImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 369);
            this.Controls.Add(this.walletDataText);
            this.Controls.Add(this.importWallet);
            this.Controls.Add(this.loadByQR);
            this.Name = "WalletImport";
            this.Text = " ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button loadByQR;
        private System.Windows.Forms.Button importWallet;
        private System.Windows.Forms.TextBox walletDataText;
    }
}