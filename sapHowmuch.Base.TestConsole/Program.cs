using System;

namespace sapHowmuch.Base.TestConsole
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				Console.WriteLine(SapStream.DICompany.CompanyName);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadLine();
		}
	}
}