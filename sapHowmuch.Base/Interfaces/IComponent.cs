namespace sapHowmuch.Base.Interfaces
{
	/// <summary>
	/// This provides interfaces to the <c>Component</c> class.
	/// </summary>
	public interface IComponent
	{
		/// <summary>
		/// Set component to register up.
		/// </summary>
		/// <param name="registerComponent"></param>
		void Setup(IRegisterComponent registerComponent);
	}
}