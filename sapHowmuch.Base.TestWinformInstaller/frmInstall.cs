using sapHowmuch.Base.Enums;
using sapHowmuch.Base.Helpers;
using sapHowmuch.Base.Installer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sapHowmuch.Base.TestWinformInstaller
{
	public partial class frmInstall : Form
	{
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		public frmInstall()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			// TODO: install 과정
		}

		private void Install(string installPath, string dllPath)
		{
			try
			{
				// TODO: Addon name, description, version 정보 리소스
				InstallAPIWrapper.Init(installPath);
				Environment.CurrentDirectory = dllPath;

				if (!Directory.Exists(installPath))
				{
					Directory.CreateDirectory(installPath);
				}
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"Install failed {Environment.NewLine}{ex.Message}");
				Application.Exit();
			}
		}

		private void ExtractFile(Assembly assembly, string installPath, string resourceFile)
		{
			try
			{
				string[] fileList = assembly.GetManifestResourceNames();

				foreach (var file in fileList)
				{
					string fileName = string.Empty;

					if (resourceFile.Contains($".{AddonFileType.InstallFile.ToString()}"))
					{
						var fileType = $".{AddonFileType.InstallFile.ToString()}.";
						var nameIndex = resourceFile.IndexOf(fileType);

						fileName = resourceFile.Substring(nameIndex + fileType.Length, resourceFile.Length - nameIndex - fileType.Length);
					}
					else if (resourceFile.Contains(".ReportFile."))
					{
						var fileType = $".{AddonFileType.ReportFile.ToString()}.";
						var nameIndex = resourceFile.IndexOf(fileType);

						fileName = resourceFile.Substring(nameIndex + fileType.Length, resourceFile.Length - nameIndex - fileType.Length);

					}
					else if (resourceFile.Contains(".ExcelFile."))
					{
						var fileType = $".{AddonFileType.ExcelFile.ToString()}.";
						var nameIndex = resourceFile.IndexOf(fileType);

						fileName = resourceFile.Substring(nameIndex + fileType.Length, resourceFile.Length - nameIndex - fileType.Length);
					}

					string sourcePath = string.Format(@"{0}\{1}.tmp", installPath, fileName);
					string targetPath = string.Format(@"{0}\{1}", installPath, fileName);

					using (var stream = assembly.GetManifestResourceStream(resourceFile))
					{
						if (File.Exists(sourcePath))
						{
							File.Delete(sourcePath);
						}

						using (var fileStream = File.Create(resourceFile))
						{
							byte[] buffer = new byte[stream.Length];
							stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
							fileStream.Write(buffer, 0, Convert.ToInt32(stream.Length));

							if (File.Exists(targetPath))
							{
								File.Delete(targetPath);
							}
						}
					}

					File.Copy(sourcePath, targetPath);

					if (File.Exists(targetPath))
					{
						File.Delete(sourcePath);
					}
				}
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"Error while copying '{resourceFile}' - {ex.Message}");
				Application.Exit();
			}
		}
	}
}
