using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	public class VT0010Controller : FormController, IFormMenuItem
	{
		public override void FormCreated()
		{
			using (Form.FreezeEx())
			{
				Form.VisibleEx = true;
			}
		}

		public string MenuItemTitle => "부가세 자료집계";
		public string ParentMenuItemId => TestConstants.VATRootMenuId;
		public override string MenuItemId => "VT0010Z";
		public override int MenuItemPosition => 1;
		public override bool Unique => false;
	}
}
