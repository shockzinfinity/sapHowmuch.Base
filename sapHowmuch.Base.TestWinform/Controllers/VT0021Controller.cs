using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	public class VT0021Controller : FormController
	{
		public override void FormCreated()
		{
			using (Form.FreezeEx())
			{
				Form.VisibleEx = true;
			}
		}

		public override bool Unique => true;
	}
}