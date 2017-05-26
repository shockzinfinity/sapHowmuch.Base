using System;

namespace sapHowmuch.Base.EventArguments
{
	public interface ISapEventArgs<T>
	{
		DateTime EventFiredTime { get; }
		T DetailArg { get; }
	}
}