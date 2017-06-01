using System.Runtime.InteropServices;

namespace sapHowmuch.Base.Extensions
{
	public static class MarshalExtensions
	{
		public static void ReleaseComObject(this object instance)
		{
			if (instance != null)
			{
				try
				{
					while (Marshal.ReleaseComObject(instance) > 0) ;
				}
				catch
				{
					// NOTE : ignore exception
				}
				finally
				{
					instance = null;
				}
			}
		}
	}
}