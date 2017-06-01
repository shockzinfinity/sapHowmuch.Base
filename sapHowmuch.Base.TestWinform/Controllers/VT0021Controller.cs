using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	public class VT0021Controller : FormController, IFormMenuItem
	{
		public override void FormCreated()
		{
			using (Form.FreezeEx())
			{
				Form.VisibleEx = true;
			}
		}

		public string MenuItemTitle => "세금계산서 발행";
		public string ParentMenuItemId => TestConstants.VATRootMenuId;
		public override string MenuItemId => "VT0021Z";
		public override int MenuItemPosition => 0;
		public override bool Unique => true;
	}
}
