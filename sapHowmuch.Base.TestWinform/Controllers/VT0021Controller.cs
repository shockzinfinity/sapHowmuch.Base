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

		public override bool Unique => true;

		public IEnumerable<FormMenuItem> MenuItems => new[]
		{
			new FormMenuItem
			{
				MenuItemTitle = "세금계산서 발행",
				ParentMenuItemId = TestConstants.VATRootMenuId1,
				MenuItemId = "VT0020Z",
				MenuItemPosition = 0
			},
			new FormMenuItem
			{
				MenuItemTitle = "세금계산서 발행+",
				ParentMenuItemId = TestConstants.VATRootMenuId2,
				MenuItemId = "VT0021Z",
				MenuItemPosition = 1
			}
		};
	}
}
