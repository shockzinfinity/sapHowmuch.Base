using Autofac;

namespace sapHowmuch.Base.TestConsole
{
	public static class DependencyConfig
	{
		public static IContainer Configure()
		{
			var builder = new ContainerBuilder();

			ComponentLoader.LoadContainer(builder, ".\\", "sapHowmuch.Base.dll");

			var container = builder.Build();

			return container;
		}
	}
}