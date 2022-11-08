
namespace tbd.View
{
    partial class AccountDetails
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
            this.label1 = new System.Windows.Forms.Label();
            this.accountID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bAddress = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.balanceLbl = new System.Windows.Forms.Label();
            this.accQRImg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.accQRImg)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account ID:";
            // 
            // accountID
            // 
            this.accountID.AutoSize = true;
            this.accountID.Location = new System.Drawing.Point(184, 22);
            this.accountID.Name = "accountID";
            this.accountID.Size = new System.Drawing.Size(155, 12);
            this.accountID.TabIndex = 1;
            this.accountID.Text = "xxxxxxxxxxxxxxxxxxxxxxxxx";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "BlockChain Address:";
            // 
            // bAddress
            // 
            this.bAddress.AutoSize = true;
            this.bAddress.Location = new System.Drawing.Point(184, 63);
            this.bAddress.Name = "bAddress";
            this.bAddress.Size = new System.Drawing.Size(155, 12);
            this.bAddress.TabIndex = 3;
            this.bAddress.Text = "xxxxxxxxxxxxxxxxxxxxxxxxx";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Balance:";
            // 
            // balanceLbl
            // 
            this.balanceLbl.AutoSize = true;
            this.balanceLbl.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.balanceLbl.Location = new System.Drawing.Point(64, 150);
            this.balanceLbl.Name = "balanceLbl";
            this.balanceLbl.Size = new System.Drawing.Size(63, 33);
            this.balanceLbl.TabIndex = 5;
            this.balanceLbl.Text = "0.0";
            // 
            // accQRImg
            // 
            this.accQRImg.Location = new System.Drawing.Point(186, 150);
            this.accQRImg.Name = "accQRImg";
            this.accQRImg.Size = new System.Drawing.Size(240, 240);
            this.accQRImg.TabIndex = 6;
            this.accQRImg.TabStop = false;
            // 
            // AccountDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 450);
            this.Controls.Add(this.accQRImg);
            this.Controls.Add(this.balanceLbl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.accountID);
            this.Controls.Add(this.label1);
            this.Name = "AccountDetails";
            this.Text = "AccountDetails";
            ((System.ComponentModel.ISupportInitialize)(this.accQRImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label accountID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label bAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label balanceLbl;
        private System.Windows.Forms.PictureBox accQRImg;
    }
}