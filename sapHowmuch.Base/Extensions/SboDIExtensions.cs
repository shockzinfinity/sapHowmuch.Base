using System.Collections.Generic;

namespace sapHowmuch.Base.Extensions
{
	public static class SboDIExtensions
	{
		public static IEnumerable<SAPbobsCOM.Field> AsEnumerable(this SAPbobsCOM.Fields fields)
		{
			foreach (var item in fields)
			{
				yield return (SAPbobsCOM.Field)item;
			}
		}
	}
}