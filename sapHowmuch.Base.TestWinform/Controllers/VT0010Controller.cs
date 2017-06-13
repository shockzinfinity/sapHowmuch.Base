using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using sapHowmuch.Base.Helpers;
using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	public class VT0010Controller : FormController
	{
		protected override void OnFormCreate()
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
				Debug.WriteLine($"EventType: {ev.DetailArg.EventType.ToString()}");
				Debug.WriteLine($"FiredTime: {ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
				Debug.WriteLine($"Form UniqueId: {ev.DetailArg.FormUID}");
				Debug.WriteLine($"FormTypeEx: {ev.DetailArg.FormTypeEx}");
				Debug.WriteLine($"Item UniqueId: {ev.DetailArg.ItemUID}");
				Debug.WriteLine(Environment.NewLine);
			});
		}

		#region derived class dispose implementation

		private bool _disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				// free any other managed objects here.
			}

			// free any unmanaged objects here.

			sapHowmuchLogger.Debug($"{GetType().Name} Dispose is called.");

			_disposed = true;

			base.Dispose(disposing);
		}

		~VT0010Controller()
		{
			sapHowmuchLogger.Debug($"{GetType().Name} Destruct method is called.");
			Dispose(false);
		}

		#endregion derived class dispose implementation
	}
}