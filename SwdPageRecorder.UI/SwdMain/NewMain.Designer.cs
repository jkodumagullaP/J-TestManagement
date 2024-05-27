namespace CTWebsite.UI.SwdMain
{
    partial class NewMain
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
            this.txtBrowserUrl = new System.Windows.Forms.TextBox();
            this.btnStartVisualSearch = new System.Windows.Forms.Button();
            this.txtVisualSearchResult = new System.Windows.Forms.TextBox();
            this.pnlLoadingBar = new System.Windows.Forms.ProgressBar();
            this.ddlWindows = new System.Windows.Forms.ComboBox();
            this.ddlFrames = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtBrowserUrl
            // 
            this.txtBrowserUrl.Location = new System.Drawing.Point(440, 202);
            this.txtBrowserUrl.Name = "txtBrowserUrl";
            this.txtBrowserUrl.Size = new System.Drawing.Size(100, 22);
            this.txtBrowserUrl.TabIndex = 0;
            this.txtBrowserUrl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtBrowserUrl_KeyUp);
            // 
            // btnStartVisualSearch
            // 
            this.btnStartVisualSearch.Location = new System.Drawing.Point(421, 265);
            this.btnStartVisualSearch.Name = "btnStartVisualSearch";
            this.btnStartVisualSearch.Size = new System.Drawing.Size(75, 23);
            this.btnStartVisualSearch.TabIndex = 1;
            this.btnStartVisualSearch.Text = "button1";
            this.btnStartVisualSearch.UseVisualStyleBackColor = true;
            this.btnStartVisualSearch.Click += new System.EventHandler(this.btnStartVisualSearch_Click);
            // 
            // txtVisualSearchResult
            // 
            this.txtVisualSearchResult.Location = new System.Drawing.Point(571, 265);
            this.txtVisualSearchResult.Name = "txtVisualSearchResult";
            this.txtVisualSearchResult.Size = new System.Drawing.Size(100, 22);
            this.txtVisualSearchResult.TabIndex = 2;
            // 
            // pnlLoadingBar
            // 
            this.pnlLoadingBar.Location = new System.Drawing.Point(495, 394);
            this.pnlLoadingBar.Name = "pnlLoadingBar";
            this.pnlLoadingBar.Size = new System.Drawing.Size(100, 23);
            this.pnlLoadingBar.TabIndex = 3;
            // 
            // ddlWindows
            // 
            this.ddlWindows.FormattingEnabled = true;
            this.ddlWindows.Location = new System.Drawing.Point(579, 313);
            this.ddlWindows.Name = "ddlWindows";
            this.ddlWindows.Size = new System.Drawing.Size(121, 24);
            this.ddlWindows.TabIndex = 4;
            // 
            // ddlFrames
            // 
            this.ddlFrames.FormattingEnabled = true;
            this.ddlFrames.Location = new System.Drawing.Point(340, 213);
            this.ddlFrames.Name = "ddlFrames";
            this.ddlFrames.Size = new System.Drawing.Size(121, 24);
            this.ddlFrames.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(363, 141);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // NewMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ddlFrames);
            this.Controls.Add(this.ddlWindows);
            this.Controls.Add(this.pnlLoadingBar);
            this.Controls.Add(this.txtVisualSearchResult);
            this.Controls.Add(this.btnStartVisualSearch);
            this.Controls.Add(this.txtBrowserUrl);
            this.Name = "NewMain";
            this.Text = "NewMain";
            this.Load += new System.EventHandler(this.NewMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBrowserUrl;
        private System.Windows.Forms.Button btnStartVisualSearch;
        private System.Windows.Forms.TextBox txtVisualSearchResult;
        private System.Windows.Forms.ProgressBar pnlLoadingBar;
        private System.Windows.Forms.ComboBox ddlWindows;
        private System.Windows.Forms.ComboBox ddlFrames;
        private System.Windows.Forms.Button button1;
    }
}