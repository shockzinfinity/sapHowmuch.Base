using System;
using System.Reactive.Linq;
using sapHowmuch.Base.EventArguments;
using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using System.Diagnostics;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	public class VT0010Controller : FormController
	{
		protected override void FormCreated()
		{
			using (Form.FreezeEx())
			{
				SubscribeTest();
				Form.VisibleEx = true;
			}
		}

		private void SubscribeTest()
		{
			ItemEventStream.Subscribe(ev =>
			{
				Debug.WriteLine(ev.DetailArg.EventType.ToString());
				Debug.WriteLine(ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
				Debug.WriteLine(ev.FormUid);
				Debug.WriteLine(ev.DetailArg.FormTypeEx);
				Debug.WriteLine(ev.DetailArg.ItemUID);
				Debug.WriteLine(Environment.NewLine);
			});
		}
	}
}