namespace KTASimulator
{
    partial class SimulatorMainUI
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnOpenPriceFile = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnFindFile = new System.Windows.Forms.Button();
            this.btnRealTime = new System.Windows.Forms.Button();
            this.btnIntervalRun = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMnemonic = new System.Windows.Forms.TextBox();
            this.txtBidPx = new System.Windows.Forms.TextBox();
            this.lblBidPx = new System.Windows.Forms.Label();
            this.txtBidSz = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOfferSz = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOfferPx = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTradeSz = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTradePx = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnPxUpdate = new System.Windows.Forms.Button();
            //((System.ComponentModel.ISupportInitialize)(this.txtFilePath.Properties)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnOpenPriceFile
            // 
            this.btnOpenPriceFile.Location = new System.Drawing.Point(360, 28);
            this.btnOpenPriceFile.Name = "btnOpenPriceFile";
            this.btnOpenPriceFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenPriceFile.TabIndex = 0;
            this.btnOpenPriceFile.Text = "Open";
            this.btnOpenPriceFile.Click += new System.EventHandler(this.btnOpenPriceFile_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(26, 31);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(251, 20);
            this.txtFilePath.TabIndex = 1;
            // 
            // btnFindFile
            // 
            this.btnFindFile.Location = new System.Drawing.Point(283, 28);
            this.btnFindFile.Name = "btnFindFile";
            this.btnFindFile.Size = new System.Drawing.Size(51, 23);
            this.btnFindFile.TabIndex = 2;
            this.btnFindFile.Text = "Find";
            this.btnFindFile.Click += new System.EventHandler(this.btnFindFile_Click);
            // 
            // btnRealTime
            // 
            this.btnRealTime.Location = new System.Drawing.Point(360, 70);
            this.btnRealTime.Name = "btnRealTime";
            this.btnRealTime.Size = new System.Drawing.Size(75, 23);
            this.btnRealTime.TabIndex = 4;
            this.btnRealTime.Text = "RealTime";
            this.btnRealTime.Click += new System.EventHandler(this.btnRealTime_Click);
            // 
            // btnIntervalRun
            // 
            this.btnIntervalRun.Location = new System.Drawing.Point(279, 70);
            this.btnIntervalRun.Name = "btnIntervalRun";
            this.btnIntervalRun.Size = new System.Drawing.Size(75, 23);
            this.btnIntervalRun.TabIndex = 5;
            this.btnIntervalRun.Text = "Interval";
            this.btnIntervalRun.Click += new System.EventHandler(this.btnIntervalRun_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Interval milli sec";
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(155, 70);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(100, 21);
            this.txtInterval.TabIndex = 7;
            this.txtInterval.Text = "10";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(517, 164);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnIntervalRun);
            this.tabPage1.Controls.Add(this.txtInterval);
            this.tabPage1.Controls.Add(this.btnOpenPriceFile);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtFilePath);
            this.tabPage1.Controls.Add(this.btnFindFile);
            this.tabPage1.Controls.Add(this.btnRealTime);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(509, 138);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "CannedData";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnPxUpdate);
            this.tabPage2.Controls.Add(this.txtTradeSz);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.txtTradePx);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.txtOfferSz);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.txtOfferPx);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.txtBidSz);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.txtBidPx);
            this.tabPage2.Controls.Add(this.lblBidPx);
            this.tabPage2.Controls.Add(this.txtMnemonic);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(509, 138);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SingleUpdate";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Mnemonic";
            // 
            // txtMnemonic
            // 
            this.txtMnemonic.Location = new System.Drawing.Point(69, 17);
            this.txtMnemonic.Name = "txtMnemonic";
            this.txtMnemonic.Size = new System.Drawing.Size(205, 21);
            this.txtMnemonic.TabIndex = 1;
            // 
            // txtBidPx
            // 
            this.txtBidPx.Location = new System.Drawing.Point(69, 46);
            this.txtBidPx.Name = "txtBidPx";
            this.txtBidPx.Size = new System.Drawing.Size(64, 21);
            this.txtBidPx.TabIndex = 3;
            this.txtBidPx.Text = "0";
            // 
            // lblBidPx
            // 
            this.lblBidPx.AutoSize = true;
            this.lblBidPx.Location = new System.Drawing.Point(9, 49);
            this.lblBidPx.Name = "lblBidPx";
            this.lblBidPx.Size = new System.Drawing.Size(33, 13);
            this.lblBidPx.TabIndex = 2;
            this.lblBidPx.Text = "BidPx";
            // 
            // txtBidSz
            // 
            this.txtBidSz.Location = new System.Drawing.Point(210, 46);
            this.txtBidSz.Name = "txtBidSz";
            this.txtBidSz.Size = new System.Drawing.Size(64, 21);
            this.txtBidSz.TabIndex = 5;
            this.txtBidSz.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(153, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "BidSz";
            // 
            // txtOfferSz
            // 
            this.txtOfferSz.Location = new System.Drawing.Point(210, 74);
            this.txtOfferSz.Name = "txtOfferSz";
            this.txtOfferSz.Size = new System.Drawing.Size(64, 21);
            this.txtOfferSz.TabIndex = 9;
            this.txtOfferSz.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(153, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "OfferSz";
            // 
            // txtOfferPx
            // 
            this.txtOfferPx.Location = new System.Drawing.Point(69, 74);
            this.txtOfferPx.Name = "txtOfferPx";
            this.txtOfferPx.Size = new System.Drawing.Size(64, 21);
            this.txtOfferPx.TabIndex = 7;
            this.txtOfferPx.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "OfferPx";
            // 
            // txtTradeSz
            // 
            this.txtTradeSz.Location = new System.Drawing.Point(210, 100);
            this.txtTradeSz.Name = "txtTradeSz";
            this.txtTradeSz.Size = new System.Drawing.Size(64, 21);
            this.txtTradeSz.TabIndex = 13;
            this.txtTradeSz.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(153, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "TradeSz";
            // 
            // txtTradePx
            // 
            this.txtTradePx.Location = new System.Drawing.Point(69, 100);
            this.txtTradePx.Name = "txtTradePx";
            this.txtTradePx.Size = new System.Drawing.Size(64, 21);
            this.txtTradePx.TabIndex = 11;
            this.txtTradePx.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "TradePx";
            // 
            // btnPxUpdate
            // 
            this.btnPxUpdate.Location = new System.Drawing.Point(304, 17);
            this.btnPxUpdate.Name = "btnPxUpdate";
            this.btnPxUpdate.Size = new System.Drawing.Size(51, 23);
            this.btnPxUpdate.TabIndex = 14;
            this.btnPxUpdate.Text = "Update";
            this.btnPxUpdate.Click += new System.EventHandler(this.btnPxUpdate_Click);
            // 
            // SimulatorMainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 164);
            this.Controls.Add(this.tabControl1);
            this.Name = "SimulatorMainUI";
            this.Text = "SimulatorMainUI";
            //((System.ComponentModel.ISupportInitialize)(this.txtFilePath.Properties)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnOpenPriceFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnFindFile;
        private System.Windows.Forms.Button btnRealTime;
        private System.Windows.Forms.Button btnIntervalRun;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnPxUpdate;
        private System.Windows.Forms.TextBox txtTradeSz;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTradePx;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtOfferSz;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOfferPx;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBidSz;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBidPx;
        private System.Windows.Forms.Label lblBidPx;
        private System.Windows.Forms.TextBox txtMnemonic;
        private System.Windows.Forms.Label label2;
    }
}