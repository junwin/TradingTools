namespace KTACQG
{
    partial class CQGFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CQGFrm));
            this.btnSubscribe = new System.Windows.Forms.Button();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnTestProbe = new System.Windows.Forms.Button();
            this.txtFunc = new System.Windows.Forms.TextBox();
            this.chkUseThrottle = new System.Windows.Forms.CheckBox();
            this.txtTestField1 = new System.Windows.Forms.TextBox();
            this.txtTestField2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSubscribe
            // 
            this.btnSubscribe.Location = new System.Drawing.Point(147, 12);
            this.btnSubscribe.Name = "btnSubscribe";
            this.btnSubscribe.Size = new System.Drawing.Size(84, 23);
            this.btnSubscribe.TabIndex = 1;
            this.btnSubscribe.Text = "Subscribe";
            this.btnSubscribe.UseVisualStyleBackColor = true;
            this.btnSubscribe.Click += new System.EventHandler(this.btnSubscribe_Click);
            // 
            // txtProduct
            // 
            this.txtProduct.Location = new System.Drawing.Point(1, 12);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(126, 20);
            this.txtProduct.TabIndex = 2;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(1, 41);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(230, 80);
            this.txtMessage.TabIndex = 5;
            // 
            // btnTestProbe
            // 
            this.btnTestProbe.Location = new System.Drawing.Point(12, 224);
            this.btnTestProbe.Name = "btnTestProbe";
            this.btnTestProbe.Size = new System.Drawing.Size(84, 23);
            this.btnTestProbe.TabIndex = 6;
            this.btnTestProbe.Text = "Test";
            this.btnTestProbe.UseVisualStyleBackColor = true;
            this.btnTestProbe.Click += new System.EventHandler(this.btnTestProbe_Click);
            // 
            // txtFunc
            // 
            this.txtFunc.Enabled = false;
            this.txtFunc.Location = new System.Drawing.Point(97, 127);
            this.txtFunc.Name = "txtFunc";
            this.txtFunc.Size = new System.Drawing.Size(91, 20);
            this.txtFunc.TabIndex = 7;
            this.txtFunc.Text = "1000";
            // 
            // chkUseThrottle
            // 
            this.chkUseThrottle.AutoSize = true;
            this.chkUseThrottle.Enabled = false;
            this.chkUseThrottle.Location = new System.Drawing.Point(7, 129);
            this.chkUseThrottle.Name = "chkUseThrottle";
            this.chkUseThrottle.Size = new System.Drawing.Size(84, 17);
            this.chkUseThrottle.TabIndex = 8;
            this.chkUseThrottle.Text = "Use Throttle";
            this.chkUseThrottle.UseVisualStyleBackColor = true;
            this.chkUseThrottle.CheckedChanged += new System.EventHandler(this.chkUseThrottle_CheckedChanged);
            // 
            // txtTestField1
            // 
            this.txtTestField1.Enabled = false;
            this.txtTestField1.Location = new System.Drawing.Point(12, 266);
            this.txtTestField1.Name = "txtTestField1";
            this.txtTestField1.Size = new System.Drawing.Size(91, 20);
            this.txtTestField1.TabIndex = 9;
            this.txtTestField1.Text = "EP";
            // 
            // txtTestField2
            // 
            this.txtTestField2.Enabled = false;
            this.txtTestField2.Location = new System.Drawing.Point(12, 292);
            this.txtTestField2.Name = "txtTestField2";
            this.txtTestField2.Size = new System.Drawing.Size(91, 20);
            this.txtTestField2.TabIndex = 10;
            // 
            // CQGFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 122);
            this.ControlBox = false;
            this.Controls.Add(this.txtTestField2);
            this.Controls.Add(this.txtTestField1);
            this.Controls.Add(this.chkUseThrottle);
            this.Controls.Add(this.txtFunc);
            this.Controls.Add(this.btnTestProbe);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtProduct);
            this.Controls.Add(this.btnSubscribe);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CQGFrm";
            this.Text = "CQG K2 Driver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CQGFrm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CQGFrm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSubscribe;
        private System.Windows.Forms.TextBox txtProduct;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnTestProbe;
        private System.Windows.Forms.TextBox txtFunc;
        private System.Windows.Forms.CheckBox chkUseThrottle;
        private System.Windows.Forms.TextBox txtTestField1;
        private System.Windows.Forms.TextBox txtTestField2;
    }
}