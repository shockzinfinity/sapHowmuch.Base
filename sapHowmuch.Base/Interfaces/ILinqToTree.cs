using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM;

namespace sapHowmuch.Base.Interfaces
{
	/// <summary>
	/// Defines an adapter that must be implemented in order to use the LinqToTree extension methods
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ILinqToTree<T>
	{
		/// <summary>
		/// obtains all the children of the item.
		/// </summary>
		/// <returns></returns>
		IEnumerable<T> Children();

		/// <summary>
		/// The parent of the item.
		/// </summary>
		T Parent { get; }
	}

	public class MenuItemTreeAdapter : ILinqToTree<SAPbouiCOM.MenuItem>
	{
		public MenuItem Parent
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerable<MenuItem> Children()
		{
			throw new NotImplementedException();
		}
	}
}
