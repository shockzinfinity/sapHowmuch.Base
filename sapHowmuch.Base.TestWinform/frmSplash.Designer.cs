namespace sapHowmuch.Base.Forms
{
	partial class frmSplash
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
			this.lblStatusInfo = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblStatusInfo
			// 
			this.lblStatusInfo.AutoSize = true;
			this.lblStatusInfo.BackColor = System.Drawing.Color.Transparent;
			this.lblStatusInfo.Location = new System.Drawing.Point(17, 49);
			this.lblStatusInfo.Name = "lblStatusInfo";
			this.lblStatusInfo.Size = new System.Drawing.Size(62, 12);
			this.lblStatusInfo.TabIndex = 0;
			this.lblStatusInfo.Text = "Loading...";
			// 
			// frmSplash
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::sapHowmuch.Base.TestWinform.Properties.Resources.Splash;
			this.ClientSize = new System.Drawing.Size(389, 69);
			this.Controls.Add(this.lblStatusInfo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "frmSplash";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "frmSplash";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblStatusInfo;
	}
}