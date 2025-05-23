﻿using System.Drawing;
using System.Windows.Forms;
using System;

namespace WebviewAlberto
{
    partial class Form5
    {
        private System.ComponentModel.IContainer components = null;
        private Microsoft.Web.WebView2.WinForms.WebView2 browser;
        private Button button1;
        private Button button2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.browser = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.button1 = new Button();
            this.button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.browser)).BeginInit();
            this.SuspendLayout();
            
            this.browser.AllowExternalDrop = true;
            this.browser.CreationProperties = null;
            this.browser.DefaultBackgroundColor = Color.White;
            this.browser.Dock = DockStyle.Fill;
            this.browser.Location = new Point(0, 0);
            this.browser.Name = "webView21";
            this.browser.Size = new Size(800, 450);
            this.browser.TabIndex = 0;
            this.browser.ZoomFactor = 1.0;
            
            this.button1.BackColor = Color.FromArgb(220, 38, 38);
            this.button1.ForeColor = SystemColors.ButtonFace;
            this.button1.Location = new Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new Size(122, 61);
            this.button1.TabIndex = 1;
            this.button1.Text = "Home";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.button2.BackColor = Color.FromArgb(220, 38, 38);
            this.button2.ForeColor = SystemColors.ButtonFace;
            this.button2.Location = new Point(674, 12);
            this.button2.Name = "button2";
            this.button2.Size = new Size(108, 61);
            this.button2.TabIndex = 2;
            this.button2.Text = "Refresh";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new EventHandler(this.button2_Click);
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new SizeF(8.0F, 20.0F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.browser);
            this.Name = "Form5";
            this.Text = "Form5";
            this.Load += new EventHandler(this.Form5_Load);
            ((System.ComponentModel.ISupportInitialize)(this.browser)).EndInit();
            this.ResumeLayout(false);
        }
    }

}