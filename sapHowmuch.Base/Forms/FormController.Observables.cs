using sapHowmuch.Base.EventArguments;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace sapHowmuch.Base.Forms
{
	partial class FormController
	{
		// TODO: 나머지 이벤트 스트림 추가
		protected IObservable<SapItemEventArgs> ItemEventStream { get; set; }

		private void MakeFormStream()
		{
			ItemEventStream = SapStream.ItemEventStream.Where(e => e.DetailArg.FormUID == this.UniqueId);
		}
	}
}