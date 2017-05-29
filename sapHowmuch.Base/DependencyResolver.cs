using Autofac;
using sapHowmuch.Base.Interfaces;
using System.ComponentModel.Composition;

namespace sapHowmuch.Base
{
	[Export(typeof(IComponent))]
	public class DependencyResolver : IComponent
	{
		#region IComponent implementation

		public void Setup(IRegisterComponent registerComponent)
		{
			registerComponent.Builder.RegisterType<SapStream>()
				.As<ISapStream>()
				.PropertiesAutowired()
				.InstancePerLifetimeScope();
		}

		#endregion IComponent implementation
	}
}