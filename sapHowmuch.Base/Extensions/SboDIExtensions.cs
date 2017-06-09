using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
