﻿using System;
using System.Windows.Forms;

partial class RegistroQr : Form
{
    private Microsoft.Web.WebView2.WinForms.WebView2 browser;
    private System.ComponentModel.IContainer components = null;

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
        ((System.ComponentModel.ISupportInitialize)(this.browser)).BeginInit();
        this.SuspendLayout();
        // 
        // browser
        // 
        this.browser.AllowExternalDrop = true;
        this.browser.CreationProperties = null;
        this.browser.DefaultBackgroundColor = System.Drawing.Color.White;
        this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
        this.browser.Location = new System.Drawing.Point(0, 0);
        this.browser.Name = "browser";
        this.browser.Size = new System.Drawing.Size(800, 600);
        this.browser.TabIndex = 0;
        this.browser.ZoomFactor = 1D;
        // 
        // Tawk
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(500, 500);
        this.Controls.Add(this.browser);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // Sin bordes
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen; // Centrado
        this.Opacity = 0.95; // Opcional: efecto translúcido
        this.Name = "RegistroQr";
        this.Text = "RegistroQr";
        this.Load += new System.EventHandler(this.RegistroQr_Load);
        ((System.ComponentModel.ISupportInitialize)(this.browser)).EndInit();
        this.ResumeLayout(false);

        // Manejo de cierre con Escape
        this.KeyDown += RegistroQr_KeyDown;

        this.Deactivate += new EventHandler(this.RegistroQr_Deactivate);
    }

    private void RegistroQr_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            this.Close(); // Cierra el popup al presionar Escape
        }
    }
}
