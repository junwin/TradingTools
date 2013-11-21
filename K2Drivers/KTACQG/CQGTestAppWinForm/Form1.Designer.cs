namespace CQGTestAppWinForm
{
    partial class Form1
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
            this.btnCQGStartTest = new System.Windows.Forms.Button();
            this.btnReqBar = new System.Windows.Forms.Button();
            this.btnStopCQG = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCQGStartTest
            // 
            this.btnCQGStartTest.Location = new System.Drawing.Point(58, 74);
            this.btnCQGStartTest.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCQGStartTest.Name = "btnCQGStartTest";
            this.btnCQGStartTest.Size = new System.Drawing.Size(112, 35);
            this.btnCQGStartTest.TabIndex = 0;
            this.btnCQGStartTest.Text = "Start CGQ";
            this.btnCQGStartTest.UseVisualStyleBackColor = true;
            this.btnCQGStartTest.Click += new System.EventHandler(this.btnCQGStartTest_Click);
            // 
            // btnReqBar
            // 
            this.btnReqBar.Location = new System.Drawing.Point(58, 139);
            this.btnReqBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReqBar.Name = "btnReqBar";
            this.btnReqBar.Size = new System.Drawing.Size(112, 35);
            this.btnReqBar.TabIndex = 1;
            this.btnReqBar.Text = "Request Bar Data";
            this.btnReqBar.UseVisualStyleBackColor = true;
            this.btnReqBar.Click += new System.EventHandler(this.btnReqBar_Click);
            // 
            // btnStopCQG
            // 
            this.btnStopCQG.Location = new System.Drawing.Point(211, 74);
            this.btnStopCQG.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStopCQG.Name = "btnStopCQG";
            this.btnStopCQG.Size = new System.Drawing.Size(112, 35);
            this.btnStopCQG.TabIndex = 2;
            this.btnStopCQG.Text = "Stop CGQ";
            this.btnStopCQG.UseVisualStyleBackColor = true;
            this.btnStopCQG.Click += new System.EventHandler(this.btnStopCQG_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 402);
            this.Controls.Add(this.btnStopCQG);
            this.Controls.Add(this.btnReqBar);
            this.Controls.Add(this.btnCQGStartTest);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "CQG Runner";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCQGStartTest;
        private System.Windows.Forms.Button btnReqBar;
        private System.Windows.Forms.Button btnStopCQG;
    }
}

