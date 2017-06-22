using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace sapHowmuch.Base.Forms
{
	public class Splasher
	{
		private static Form _splashForm = null;
		private static Thread _splashThread = null;
		private static string _tempStatus = string.Empty;

		private delegate void SplashStatusChanged(string statusInfo);

		public static string Status
		{
			set
			{
				if (_splashForm == null)
				{
					_tempStatus = value;
					return;
				}

				if (_splashForm.InvokeRequired)
				{
					_splashForm.Invoke(new SplashStatusChanged(delegate (string str)
					{
						(_splashForm as ISplash).SetStatusInfo(str);
					}), new object[] { value });
				}
				else
				{
					(_splashForm as ISplash)?.SetStatusInfo(_tempStatus);
				}
			}
		}

		public static void Show(Type formType)
		{
			if (_splashThread != null) return;

			if (formType == null)
			{
				throw new ArgumentNullException(nameof(formType));
			}

			if (_splashForm == null)
			{
				_splashForm = formType.InvokeMember(string.Empty, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null) as Form;
			}

			if (!string.IsNullOrWhiteSpace(_tempStatus))
			{
				(_splashForm as ISplash)?.SetStatusInfo(_tempStatus);
			}

			_splashThread = new Thread(new ThreadStart(delegate ()
			{
				Application.Run(_splashForm);
			}));
			_splashThread.IsBackground = true;
			_splashThread.SetApartmentState(ApartmentState.STA);
			_splashThread.Start();
		}

		public static void Close()
		{
			if (_splashForm == null || _splashThread == null) return;

			try
			{
				_splashForm.Invoke(new MethodInvoker(_splashForm.Close));
			}
			catch { /* ignore */			}

			_splashForm = null;
			_splashThread = null;
		}
	}
}