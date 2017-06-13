using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using sapHowmuch.Base.Helpers;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	public class VT0021Controller : FormController
	{
		protected override void OnFormCreate()
		{
			using (Form.FreezeEx())
			{
				Form.VisibleEx = true;
			}
		}

		public override bool Unique => true;

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

		~VT0021Controller()
		{
			sapHowmuchLogger.Debug($"{GetType().Name} Destruct method is called.");
			Dispose(false);
		}

		#endregion derived class dispose implementation
	}
}