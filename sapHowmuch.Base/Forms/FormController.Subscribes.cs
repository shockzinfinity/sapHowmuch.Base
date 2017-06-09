using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using sapHowmuch.Base.EventArguments;

namespace sapHowmuch.Base.Forms
{
	partial class FormController
	{
		#region event stream subscribes

		private void SubscribeToStream()
		{
			SapStream.ItemEventStream
				.Where(e => e.FormUid == this.UniqueId)
				.Subscribe(ev =>
				{
					Debug.WriteLine(ev.EventFiredTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
					Debug.WriteLine(ev.FormUid);
					Debug.WriteLine(ev.DetailArg.FormTypeEx);
					Debug.WriteLine(ev.DetailArg.ItemUID);
					Debug.WriteLine(ev.DetailArg.EventType.ToString());
				});
		}

		private void MakeFormStream()
		{
			ItemEventStream = SapStream.ItemEventStream.Where(e => e.FormUid == this.UniqueId);
		}

		#endregion

		#region observables

		protected IObservable<SapItemEventArgs> ItemEventStream { get; set; }

		#endregion
	}
}
