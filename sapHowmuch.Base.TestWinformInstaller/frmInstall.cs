using sapHowmuch.Base.Enums;
using sapHowmuch.Base.Helpers;
using sapHowmuch.Base.Installer;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

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
		private int _resourcesCount;
		private int _percent;

		public frmInstall()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			var addonInfoType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(a => !a.IsInterface && a.BaseType == typeof(AddonInfo));
			_addonInfo = Activator.CreateInstance(addonInfoType) as AddonInfo;
			var commandArgs = Environment.GetCommandLineArgs();

			lblAddonVersion.Text = _addonInfo.AddonVersion.ToString();
			lblAddonName.Text = _addonInfo.AddonName;
			lblDescription.Text = _addonInfo.PartnerContact;
			lblInstallPath.Text = _addonInfo.InstallPath;

			try
			{
				if (commandArgs.Length == 2)
				{
					if (commandArgs[1] == "/u")
					{
						Uninstall();
						Application.Exit();
					}
					else if (commandArgs[1] == "/g")
					{
						var knownTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(AddonInfo)).ToArray();
						var settings = new XmlWriterSettings
						{
							Indent = true,
							OmitXmlDeclaration = false,
							Encoding = Encoding.GetEncoding("UTF-16")
						};

						var namespaces = new XmlSerializerNamespaces();
						namespaces.Add(string.Empty, string.Empty);

						XmlSerializer serializer = new XmlSerializer(typeof(AddonInfo), knownTypes);

						using (var xmlWriter = XmlWriter.Create($@"{Environment.CurrentDirectory}\{_addonInfo.AddonName}.xml", settings))
						{
							serializer.Serialize(xmlWriter, _addonInfo, namespaces);
						}

						#region ard file generating

						Process addondatagen = new Process();
						addondatagen.StartInfo.FileName = sapHowmuchConstants.AddOnRegDataGenPath;
						addondatagen.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
						addondatagen.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						// "AddOnInfo.xml" "Version" "Installer.exe" "Installer.exe" "AddOnExecutable"
						addondatagen.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\"");

						addondatagen.Start();

						// addon name 에 따른 ard 파일 생성
						// TODO: 빌드 이후 이벤트에 /g 로 해서 실행

						#endregion ard file generating

						Application.Exit();
					}

					var elements = commandArgs[1].Split(new char[] { char.Parse("|") });
					_addonInfo.InstallPath = elements[0];

					// AddOnInstallAPI path
					// TODO: 문자열 안자르고 그냥 넣었을 때, installapi 동작하는지 여부 체크
					var pathIndex = elements[1].LastIndexOf("\\") + 1;
					_addonInfo.DllPath = elements[1].Remove(pathIndex, elements[1].Length - pathIndex);
				}
				else
				{
					// TODO: 설치실패 처리 및 로깅
					sapHowmuchLogger.Error("Failed to install");
					Application.Exit();
				}
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error($"Failed to install - {ex.Message}");
				MessageBox.Show(ex.Message);
				Application.Exit();
			}
		}

		private void Install()
		{
			try
			{
				_percent = 0;

				if (_addonInfo != null)
				{
					ProgressBarDisplay(string.Format("{0} 애드온 설치를 위해 SAP-얼마에요에 연결중입니다.", _addonInfo.AddonName), false);

					InstallAPIWrapper.Init(_addonInfo.DllPath);
					Environment.CurrentDirectory = _addonInfo.DllPath;

					ProgressBarDisplay(string.Format("설치 폴더를 확인중입니다..", new object[0]), false);
					if (!Directory.Exists(_addonInfo.InstallPath))
					{
						Directory.CreateDirectory(_addonInfo.InstallPath);
					}

					// extract files
					var assembly = Assembly.GetExecutingAssembly();

					ExtractFile(assembly, _addonInfo.InstallPath);

					if (chkRestart.Checked)
					{
						ProgressBarDisplay(string.Format("{0} 애드온을 시작하기 위해 SAP-얼마에요을 재시작합니다.", _addonInfo.AddonName), false);
						InstallAPIWrapper.RestartNeeded();
					}

					ProgressBarDisplay(string.Format(@"{0} 애드온 설치가 완료되었습니다.", _addonInfo.AddonName), true);
					InstallAPIWrapper.EndInstall("installation completed", true);
				}
				else
				{
					throw new ArgumentNullException(nameof(_addonInfo));
				}
			}
			catch (Exception ex)
			{
				ProgressBarDisplay("설치중 에러로 인해 설치를 중단하였습니다", true);

				btnInstall.UseWaitCursor = false;
				chkRestart.Enabled = true;
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
					var fileList = Assembly.GetExecutingAssembly().GetManifestResourceNames();

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
				_resourcesCount = fileList.Length;

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

					ProgressBarDisplay(string.Format("{0} 파일을 설치중입니다.", fileName), false);

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

		private void ProgressBarDisplay(string message, bool isFinal = false)
		{
			// TODO: Rx 로 변경
			try
			{
				int weight = 100 / _resourcesCount;

				_percent = _percent + weight;

				lblMessage.Text = message;

				if (isFinal)
				{
					pbarStatus.Value = 100;
				}
				else
				{
					pbarStatus.Value = _percent;
				}

				// TODO: remove do events
				Application.DoEvents();
			}
			catch (Exception ex)
			{
				sapHowmuchLogger.Error(ex.Message);
			}
		}

		private void frmInstall_MouseDown(object sender, MouseEventArgs e)
		{
			// TODO: Rx 로 변경
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void btnInstall_Click(object sender, EventArgs e)
		{
			// TODO: Rx 로 변경
			if (btnInstall.UseWaitCursor == false)
			{
				btnInstall.UseWaitCursor = true;
				chkRestart.Enabled = false;
				Install();
			}
		}
	}
}