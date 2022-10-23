
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
            this.pathofQR = new System.Windows.Forms.Label();
            this.loadByQR = new System.Windows.Forms.Button();
            this.importWallet = new System.Windows.Forms.Button();
            this.walletDataText = new System.Windows.Forms.TextBox();
            this.ImportImage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pathofQR
            // 
            this.pathofQR.AutoSize = true;
            this.pathofQR.Location = new System.Drawing.Point(10, 379);
            this.pathofQR.Name = "pathofQR";
            this.pathofQR.Size = new System.Drawing.Size(101, 12);
            this.pathofQR.TabIndex = 1;
            this.pathofQR.Text = "path of qr image";
            // 
            // loadByQR
            // 
            this.loadByQR.Location = new System.Drawing.Point(259, 374);
            this.loadByQR.Name = "loadByQR";
            this.loadByQR.Size = new System.Drawing.Size(75, 23);
            this.loadByQR.TabIndex = 3;
            this.loadByQR.Text = "find QR Image";
            this.loadByQR.UseVisualStyleBackColor = true;
            this.loadByQR.Click += new System.EventHandler(this.loadQRImage);
            // 
            // importWallet
            // 
            this.importWallet.Location = new System.Drawing.Point(202, 253);
            this.importWallet.Name = "importWallet";
            this.importWallet.Size = new System.Drawing.Size(132, 23);
            this.importWallet.TabIndex = 4;
            this.importWallet.Text = "import By Data ";
            this.importWallet.UseVisualStyleBackColor = true;
            this.importWallet.Click += new System.EventHandler(this.importThisAccount);
            // 
            // walletDataText
            // 
            this.walletDataText.Location = new System.Drawing.Point(12, 37);
            this.walletDataText.Multiline = true;
            this.walletDataText.Name = "walletDataText";
            this.walletDataText.Size = new System.Drawing.Size(465, 210);
            this.walletDataText.TabIndex = 5;
            // 
            // ImportImage
            // 
            this.ImportImage.Location = new System.Drawing.Point(358, 374);
            this.ImportImage.Name = "ImportImage";
            this.ImportImage.Size = new System.Drawing.Size(75, 23);
            this.ImportImage.TabIndex = 6;
            this.ImportImage.Text = "Import";
            this.ImportImage.UseVisualStyleBackColor = true;
            this.ImportImage.Click += new System.EventHandler(this.ImportImage_Click);
            // 
            // WalletImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 466);
            this.Controls.Add(this.ImportImage);
            this.Controls.Add(this.walletDataText);
            this.Controls.Add(this.importWallet);
            this.Controls.Add(this.loadByQR);
            this.Controls.Add(this.pathofQR);
            this.Name = "WalletImport";
            this.Text = "WalletImport";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label pathofQR;
        private System.Windows.Forms.Button loadByQR;
        private System.Windows.Forms.Button importWallet;
        private System.Windows.Forms.TextBox walletDataText;
        private System.Windows.Forms.Button ImportImage;
    }
}