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

		public override bool Unique => false;

		public IEnumerable<FormMenuItem> MenuItems => new[]
		{
			new FormMenuItem
			{
				MenuItemTitle = "부가세 자료집계",
				ParentMenuItemId = TestConstants.VATRootMenuId1,
				MenuItemId = "VT0010Z",
				MenuItemPosition = 0
			},
			new FormMenuItem
			{
				MenuItemTitle = "부가세 자료집계+",
				ParentMenuItemId = TestConstants.VATRootMenuId2,
				MenuItemId = "VT0011Z",
				MenuItemPosition = 1
			}
		};
	}
}
