namespace CAN_Buff_sniffer
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.OpenClose = new System.Windows.Forms.Button();
            this.UseWhiteList = new System.Windows.Forms.CheckBox();
            this.UseBlackList = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.LogOnly = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.FileName = new System.Windows.Forms.TextBox();
            this.LogWithMs = new System.Windows.Forms.CheckBox();
            this.LogWithTime = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.Status = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(57, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "COM10";
            // 
            // OpenClose
            // 
            this.OpenClose.Location = new System.Drawing.Point(69, 17);
            this.OpenClose.Name = "OpenClose";
            this.OpenClose.Size = new System.Drawing.Size(75, 23);
            this.OpenClose.TabIndex = 0;
            this.OpenClose.Text = "Open";
            this.OpenClose.UseVisualStyleBackColor = true;
            this.OpenClose.Click += new System.EventHandler(this.button1_Click);
            // 
            // UseWhiteList
            // 
            this.UseWhiteList.AutoSize = true;
            this.UseWhiteList.Location = new System.Drawing.Point(150, 21);
            this.UseWhiteList.Name = "UseWhiteList";
            this.UseWhiteList.Size = new System.Drawing.Size(92, 17);
            this.UseWhiteList.TabIndex = 2;
            this.UseWhiteList.Text = "Use WhiteList";
            this.UseWhiteList.UseVisualStyleBackColor = true;
            // 
            // UseBlackList
            // 
            this.UseBlackList.AutoSize = true;
            this.UseBlackList.Location = new System.Drawing.Point(248, 21);
            this.UseBlackList.Name = "UseBlackList";
            this.UseBlackList.Size = new System.Drawing.Size(91, 17);
            this.UseBlackList.TabIndex = 3;
            this.UseBlackList.Text = "Use BlackList";
            this.UseBlackList.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.LogOnly);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.FileName);
            this.groupBox1.Controls.Add(this.LogWithMs);
            this.groupBox1.Controls.Add(this.LogWithTime);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.OpenClose);
            this.groupBox1.Controls.Add(this.UseBlackList);
            this.groupBox1.Controls.Add(this.UseWhiteList);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(882, 112);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Config";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(194, 46);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "Test GPS";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // LogOnly
            // 
            this.LogOnly.AutoSize = true;
            this.LogOnly.Location = new System.Drawing.Point(273, 50);
            this.LogOnly.Name = "LogOnly";
            this.LogOnly.Size = new System.Drawing.Size(66, 17);
            this.LogOnly.TabIndex = 9;
            this.LogOnly.Text = "Log only";
            this.LogOnly.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(113, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Test CAN";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 46);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Load from file";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FileName
            // 
            this.FileName.Location = new System.Drawing.Point(531, 19);
            this.FileName.Name = "FileName";
            this.FileName.Size = new System.Drawing.Size(345, 20);
            this.FileName.TabIndex = 7;
            // 
            // LogWithMs
            // 
            this.LogWithMs.AutoSize = true;
            this.LogWithMs.Location = new System.Drawing.Point(443, 21);
            this.LogWithMs.Name = "LogWithMs";
            this.LogWithMs.Size = new System.Drawing.Size(82, 17);
            this.LogWithMs.TabIndex = 5;
            this.LogWithMs.Text = "Log with ms";
            this.LogWithMs.UseVisualStyleBackColor = true;
            // 
            // LogWithTime
            // 
            this.LogWithTime.AutoSize = true;
            this.LogWithTime.Location = new System.Drawing.Point(345, 21);
            this.LogWithTime.Name = "LogWithTime";
            this.LogWithTime.Size = new System.Drawing.Size(92, 17);
            this.LogWithTime.TabIndex = 4;
            this.LogWithTime.Text = "Log with Time";
            this.LogWithTime.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Location = new System.Drawing.Point(12, 130);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(882, 407);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 16);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(876, 388);
            this.listBox1.TabIndex = 0;
            // 
            // Status
            // 
            this.Status.AutoSize = true;
            this.Status.Location = new System.Drawing.Point(15, 542);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(31, 13);
            this.Status.TabIndex = 7;
            this.Status.Text = "VIN: ";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(113, 75);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(156, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "Get Winkers map";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 564);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "CAN Bus sniffer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button OpenClose;
        private System.Windows.Forms.CheckBox UseWhiteList;
        private System.Windows.Forms.CheckBox UseBlackList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox LogWithTime;
        private System.Windows.Forms.CheckBox LogWithMs;
        private System.Windows.Forms.TextBox FileName;
        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox LogOnly;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}

