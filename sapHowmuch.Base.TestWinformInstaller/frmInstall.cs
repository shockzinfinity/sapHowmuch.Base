using Microsoft.Win32;
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
		private AddonInfo _addonInfo;

		public frmInstall()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			var addonInfoType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(a => !a.IsInterface && a.BaseType == typeof(AddonInfo));
			var _addonInfo = Activator.CreateInstance(addonInfoType) as AddonInfo;
			var commandArgs = Environment.GetCommandLineArgs();

			try
			{
				if (commandArgs.Length == 2)
				{
					if (commandArgs[1] == "/u")
					{
						Uninstall();
						Application.Exit();
					}

					var elements = commandArgs[1].Split(new char[] { char.Parse("|") });
					_addonInfo.InstallPath = elements[0];
					_addonInfo.DllPath = elements[1];
					// TODO: check && logging
					_addonInfo.DllPath = _addonInfo.DllPath.Remove((_addonInfo.DllPath.Length - 19), 19);
				}
				else
				{
					// TODO: 설치실패 처리 및 로깅
					Application.Exit();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				Application.Exit();
			}
		}

		private void Install()
		{
			try
			{
				if (_addonInfo != null)
				{
					InstallAPIWrapper.Init(_addonInfo.DllPath);
					Environment.CurrentDirectory = _addonInfo.DllPath;

					if (!Directory.Exists(_addonInfo.InstallPath))
					{
						Directory.CreateDirectory(_addonInfo.InstallPath);
					}

					// extract files
					var assembly = Assembly.GetEntryAssembly();

					ExtractFile(assembly, _addonInfo.InstallPath);

					// TODO: restart 체크
					//InstallAPIWrapper.RestartNeeded();

					InstallAPIWrapper.EndInstall("installation completed", true);
				}
				else
				{
					throw new Exception("incorrct addon info");
				}
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"Install failed {Environment.NewLine}{ex.Message}");
				throw ex;
			}
		}

		private void Uninstall()
		{
			try
			{
				if (_addonInfo != null)
				{
					InstallAPIWrapper.Init(_addonInfo.DllPath);
					var fileList = Assembly.GetEntryAssembly().GetManifestResourceNames();

					foreach (var file in fileList)
					{
						string fileName = string.Empty;

						if (file.Contains($".{AddonFileType.InstallFile.ToString()}."))
						{
							var fileType = $".{AddonFileType.InstallFile.ToString()}.";
							var nameIndex = file.IndexOf(fileType);

							fileName = file.Substring(nameIndex + fileType.Length, file.Length - nameIndex - fileType.Length);

							if (File.Exists(fileName)) File.Delete(fileName);
						}
						else if (file.Contains($".{AddonFileType.ReportFile.ToString()}."))
						{
							var fileType = $".{AddonFileType.ReportFile.ToString()}.";
							var nameIndex = file.IndexOf(fileType);

							fileName = file.Substring(nameIndex + fileType.Length, file.Length - nameIndex - fileType.Length);

							if (File.Exists(fileName)) File.Delete(fileName);
						}
						else if (file.Contains($".{AddonFileType.ExcelFile.ToString()}."))
						{
							var fileType = $".{AddonFileType.ExcelFile.ToString()}.";
							var nameIndex = file.IndexOf(fileType);

							fileName = file.Substring(nameIndex + fileType.Length, file.Length - nameIndex - fileType.Length);

							if (File.Exists(fileName)) File.Delete(fileName);
						}
					}

					// delete folder
					try
					{
						if (Directory.Exists($@"{_addonInfo.DllPath}\{AddonFileType.ExcelFile.ToString()}"))
							Directory.Delete($@"{_addonInfo.DllPath}\{AddonFileType.ExcelFile.ToString()}");

						if (Directory.Exists($@"{_addonInfo.DllPath}\{AddonFileType.ReportFile.ToString()}"))
							Directory.Delete($@"{_addonInfo.DllPath}\{AddonFileType.ReportFile.ToString()}");

						if (Directory.Exists($@"{_addonInfo.DllPath}"))
							Directory.Delete($@"{_addonInfo.DllPath}");
					}
					catch { }

					InstallAPIWrapper.EndUninstall();
				}
				else
				{
					throw new Exception("incorrct addon info");
				}
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"Uninstall failed: {ex.Message}");
				throw ex;
			}
		}

		private void ExtractFile(Assembly assembly, string installPath)
		{
			try
			{
				string[] fileList = assembly.GetManifestResourceNames();

				foreach (var file in fileList)
				{
					string fileName = string.Empty;
					string sourcePath = string.Empty;
					string targetPath = string.Empty;

					if (file.Contains($".{AddonFileType.InstallFile.ToString()}."))
					{
						var fileType = $".{AddonFileType.InstallFile.ToString()}.";
						var nameIndex = file.IndexOf(fileType);

						fileName = file.Substring(nameIndex + fileType.Length, file.Length - nameIndex - fileType.Length);
						sourcePath = string.Format(@"{0}\{1}.tmp", installPath, fileName);
						targetPath = string.Format(@"{0}\{1}", installPath, fileName);
					}
					else if (file.Contains($".{AddonFileType.ReportFile.ToString()}."))
					{
						var fileType = $".{AddonFileType.ReportFile.ToString()}.";
						var nameIndex = file.IndexOf(fileType);

						fileName = file.Substring(nameIndex + fileType.Length, file.Length - nameIndex - fileType.Length);
						sourcePath = string.Format(@"{0}\{1}\{2}.tmp", installPath, AddonFileType.ReportFile.ToString(), fileName);
						targetPath = string.Format(@"{0}\{1}\{2}", installPath, AddonFileType.ReportFile.ToString(), fileName);
					}
					else if (file.Contains($".{AddonFileType.ExcelFile.ToString()}."))
					{
						var fileType = $".{AddonFileType.ExcelFile.ToString()}.";
						var nameIndex = file.IndexOf(fileType);

						fileName = file.Substring(nameIndex + fileType.Length, file.Length - nameIndex - fileType.Length);
						sourcePath = string.Format(@"{0}\{1}\{2}.tmp", installPath, AddonFileType.ExcelFile.ToString(), fileName);
						targetPath = string.Format(@"{0}\{1}\{2}", installPath, AddonFileType.ExcelFile.ToString(), fileName);
					}

					using (var stream = assembly.GetManifestResourceStream(file))
					{
						if (File.Exists(sourcePath))
						{
							File.Delete(sourcePath);
						}

						using (var fileStream = File.Create(file))
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
				sapHowmuchLogger.Error($"Error while copying: {ex.Message}");
				Application.Exit();
			}
		}
	}
}
