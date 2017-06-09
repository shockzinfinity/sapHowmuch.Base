using sapHowmuch.Base.EventArguments;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace sapHowmuch.Base.Forms
{
	partial class FormController
	{
		protected IObservable<SapItemEventArgs> ItemEventStream { get; set; }

		private void MakeFormStream()
		{
			ItemEventStream = SapStream.ItemEventStream.Where(e => e.DetailArg.FormUID == this.UniqueId);
		}
	}
}