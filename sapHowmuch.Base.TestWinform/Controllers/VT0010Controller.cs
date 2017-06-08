using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	public class VT0010Controller : FormController
	{
		//public VT0010Controller() : base(true)
		//{

		//}

		protected override void FormCreated()
		{
			using (Form.FreezeEx())
			{
				Form.VisibleEx = true;
			}
		}
	}
}