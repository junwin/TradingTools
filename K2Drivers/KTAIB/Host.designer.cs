namespace KTAIB
{
    partial class Host
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Host));
            this.axTws1 = new AxTWSLib.AxTws();
            ((System.ComponentModel.ISupportInitialize)(this.axTws1)).BeginInit();
            this.SuspendLayout();
            // 
            // axTws1
            // 
            this.axTws1.Enabled = true;
            this.axTws1.Location = new System.Drawing.Point(85, 25);
            this.axTws1.Name = "axTws1";
            this.axTws1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTws1.OcxState")));
            this.axTws1.Size = new System.Drawing.Size(145, 25);
            this.axTws1.TabIndex = 42;
            // 
            // Host
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 74);
            this.Controls.Add(this.axTws1);
            this.Name = "Host";
            this.Text = "Host";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TDAHost_FormClosed);
            this.Load += new System.EventHandler(this.Host_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axTws1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public AxTWSLib.AxTws axTws1;



        //private Axtdaactx.AxTDAAPIComm TDAC;
    }
}