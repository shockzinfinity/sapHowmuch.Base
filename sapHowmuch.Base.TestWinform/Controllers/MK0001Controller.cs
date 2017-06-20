using sapHowmuch.Base.Attributes;
using sapHowmuch.Base.Enums;
using sapHowmuch.Base.Extensions;
using sapHowmuch.Base.Forms;
using sapHowmuch.Base.Helpers;

namespace sapHowmuch.Base.TestWinform.Controllers
{
	internal class MK0001Controller : FormController
	{
		#region general form settings

		public override bool Unique => false;

		#endregion general form settings

		protected override void OnFormCreate()
		{
			// 초기화 로직

			using (Form.FreezeEx())
			{
				Form.VisibleEx = true;
			}
			sapHowmuchLogger.Trace($"{this.Form.Title} // UID: {this.UniqueId} // [{this.FormResource}]");
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

		~MK0001Controller()
		{
			sapHowmuchLogger.Debug($"{GetType().Name} Destruct method is called.");
			Dispose(false);
		}

		#endregion derived class dispose implementation

		#region control placeholders

		// TODO: code snippet 으로 변경 필요

		#endregion control placeholders
	}
}