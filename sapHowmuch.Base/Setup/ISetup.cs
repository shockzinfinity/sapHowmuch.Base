namespace sapHowmuch.Base.Setup
{
	public interface ISetup
	{
		int Version { get; }

		void Run();
	}
}