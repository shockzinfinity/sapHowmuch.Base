using System.Windows.Forms;

namespace sapHowmuch.Base.Forms
{
	public partial class frmSplash : Form, ISplash
	{
		public frmSplash()
		{
			InitializeComponent();
		}

		public void SetStatusInfo(string statusInfo)
		{
			lblStatusInfo.Text = statusInfo;
		}
	}
}