namespace sapHowmuch.Base.TestWinformInstaller
{
	partial class frmInstall
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			this.chkRestart = new System.Windows.Forms.CheckBox();
			this.pbarStatus = new System.Windows.Forms.ProgressBar();
			this.lblAddonName = new System.Windows.Forms.Label();
			this.lblAddonVersion = new System.Windows.Forms.Label();
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblInstallPath = new System.Windows.Forms.Label();
			this.lblMessage = new System.Windows.Forms.Label();
			this.btnInstall = new sapHowmuch.Base.TestWinformInstaller.Misc.ImageButton();
			((System.ComponentModel.ISupportInitialize)(this.btnInstall)).BeginInit();
			this.SuspendLayout();
			// 
			// chkRestart
			// 
			this.chkRestart.BackColor = System.Drawing.Color.Transparent;
			this.chkRestart.Font = new System.Drawing.Font("돋움", 9F);
			this.chkRestart.Location = new System.Drawing.Point(35, 257);
			this.chkRestart.Name = "chkRestart";
			this.chkRestart.Size = new System.Drawing.Size(273, 25);
			this.chkRestart.TabIndex = 10;
			this.chkRestart.Text = "설치완료 후 SAP-얼마에요 재시작";
			this.chkRestart.UseVisualStyleBackColor = false;
			// 
			// pbarStatus
			// 
			this.pbarStatus.Location = new System.Drawing.Point(35, 338);
			this.pbarStatus.Name = "pbarStatus";
			this.pbarStatus.Size = new System.Drawing.Size(451, 16);
			this.pbarStatus.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbarStatus.TabIndex = 17;
			// 
			// lblAddonName
			// 
			this.lblAddonName.AutoSize = true;
			this.lblAddonName.BackColor = System.Drawing.Color.Transparent;
			this.lblAddonName.Font = new System.Drawing.Font("돋움", 9F);
			this.lblAddonName.Location = new System.Drawing.Point(123, 101);
			this.lblAddonName.Name = "lblAddonName";
			this.lblAddonName.Size = new System.Drawing.Size(69, 12);
			this.lblAddonName.TabIndex = 19;
			this.lblAddonName.Text = "애드온 이름";
			// 
			// lblAddonVersion
			// 
			this.lblAddonVersion.AutoSize = true;
			this.lblAddonVersion.BackColor = System.Drawing.Color.Transparent;
			this.lblAddonVersion.Font = new System.Drawing.Font("돋움", 9F);
			this.lblAddonVersion.Location = new System.Drawing.Point(367, 101);
			this.lblAddonVersion.Name = "lblAddonVersion";
			this.lblAddonVersion.Size = new System.Drawing.Size(69, 12);
			this.lblAddonVersion.TabIndex = 20;
			this.lblAddonVersion.Text = "애드온 버젼";
			// 
			// lblDescription
			// 
			this.lblDescription.AutoSize = true;
			this.lblDescription.BackColor = System.Drawing.Color.Transparent;
			this.lblDescription.Font = new System.Drawing.Font("돋움", 9F);
			this.lblDescription.Location = new System.Drawing.Point(123, 130);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(69, 12);
			this.lblDescription.TabIndex = 21;
			this.lblDescription.Text = "애드온 설명";
			// 
			// lblInstallPath
			// 
			this.lblInstallPath.AutoSize = true;
			this.lblInstallPath.BackColor = System.Drawing.Color.Transparent;
			this.lblInstallPath.Font = new System.Drawing.Font("돋움체", 8F);
			this.lblInstallPath.Location = new System.Drawing.Point(43, 206);
			this.lblInstallPath.MaximumSize = new System.Drawing.Size(425, 0);
			this.lblInstallPath.Name = "lblInstallPath";
			this.lblInstallPath.Size = new System.Drawing.Size(53, 11);
			this.lblInstallPath.TabIndex = 22;
			this.lblInstallPath.Text = "설치경로";
			// 
			// lblMessage
			// 
			this.lblMessage.AutoSize = true;
			this.lblMessage.BackColor = System.Drawing.Color.Transparent;
			this.lblMessage.Font = new System.Drawing.Font("돋움", 9F);
			this.lblMessage.Location = new System.Drawing.Point(37, 320);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(257, 12);
			this.lblMessage.TabIndex = 23;
			this.lblMessage.Text = "애드온 설치 버튼을 누르면 설치를 시작합니다.";
			// 
			// btnInstall
			// 
			this.btnInstall.BackColor = System.Drawing.Color.Transparent;
			this.btnInstall.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btnInstall.DownImage = global::sapHowmuch.Base.TestWinformInstaller.Properties.Resources.btn_addon_install_on;
			this.btnInstall.Font = new System.Drawing.Font("돋움", 9F);
			this.btnInstall.HoverImage = global::sapHowmuch.Base.TestWinformInstaller.Properties.Resources.btn_addon_install_on;
			this.btnInstall.Location = new System.Drawing.Point(201, 367);
			this.btnInstall.Name = "btnInstall";
			this.btnInstall.NormalImage = global::sapHowmuch.Base.TestWinformInstaller.Properties.Resources.btn_addon_install;
			this.btnInstall.Size = new System.Drawing.Size(125, 31);
			this.btnInstall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.btnInstall.TabIndex = 12;
			this.btnInstall.TabStop = false;
			this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
			// 
			// frmInstall
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::sapHowmuch.Base.TestWinformInstaller.Properties.Resources.installer_background;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(515, 420);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.lblInstallPath);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.lblAddonVersion);
			this.Controls.Add(this.lblAddonName);
			this.Controls.Add(this.pbarStatus);
			this.Controls.Add(this.btnInstall);
			this.Controls.Add(this.chkRestart);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmInstall";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SAP-얼마에요 Installer";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmInstall_MouseDown);
			((System.ComponentModel.ISupportInitialize)(this.btnInstall)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		internal System.Windows.Forms.CheckBox chkRestart;
		internal TestWinformInstaller.Misc.ImageButton btnInstall;
		private System.Windows.Forms.ProgressBar pbarStatus;
		private System.Windows.Forms.Label lblAddonName;
		private System.Windows.Forms.Label lblAddonVersion;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Label lblInstallPath;
		private System.Windows.Forms.Label lblMessage;
	}
}

