using sapHowmuch.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sapHowmuch.Base.Extensions
{
	/// <summary>
	/// Defines extension methods for querying an ILinqTree
	/// </summary>
	//public static class LinqToTreeExtensions
	//{
	//	#region primary Linq methods

	//	/// <summary>
	//	/// Returns a collection of descendant elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> Descendants<T>(this ILinqToTree<T> adapter)
	//	{
	//		foreach (var child in adapter.Children())
	//		{
	//			yield return child;

	//			foreach (var grandChild in child.Descendants())
	//			{
	//				yield return grandChild;
	//			}
	//		}
	//	}

	//	/// <summary>
	//	/// Returns a collection of ancestor elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> Ancestors<T>(this ILinqToTree<T> adapter)
	//	{
	//		var parent = adapter.Parent;
	//		while (parent != null)
	//		{
	//			yield return parent;
	//			parent = parent.Parent;
	//		}
	//	}

	//	/// <summary>
	//	/// Returns a collection of child elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> Elements<T>(this ILinqToTree<T> adapter)
	//	{
	//		foreach (var child in adapter.Children())
	//		{
	//			yield return child;
	//		}
	//	}

	//	#endregion primary Linq methods

	//	#region 'AndSelf' implementations

	//	/// <summary>
	//	/// Returns a collection containing this element and all child elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> ElementsAndSelf<T>(this ILinqToTree<T> adapter)
	//	{
	//		yield return adapter;

	//		foreach (var child in adapter.Elements())
	//		{
	//			yield return child;
	//		}
	//	}

	//	/// <summary>
	//	/// Returns a collection of ancestor elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> AncestorsAndSelf<T>(this ILinqToTree<T> adapter)
	//	{
	//		yield return adapter;

	//		foreach (var child in adapter.Ancestors())
	//		{
	//			yield return child;
	//		}
	//	}

	//	/// <summary>
	//	/// Returns a collection containing this element and all descendant elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> DescendantsAndSelf<T>(this ILinqToTree<T> adapter)
	//	{
	//		yield return adapter;

	//		foreach (var child in adapter.Descendants())
	//		{
	//			yield return child;
	//		}
	//	}

	//	#endregion 'AndSelf' implementations

	//	#region Method which take type parameters

	//	/// <summary>
	//	/// Returns a collection of descendant elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> Descendants<T, K>(this ILinqToTree<T> adapter)
	//	{
	//		return adapter.Descendants().Where(i => i.Item is K);
	//	}

	//	#endregion Method which take type parameters

	//	#region Enumerable extensions

	//	/// <summary>
	//	/// Applies the given function to each of the items in the supplied
	//	/// IEnumerable.
	//	/// </summary>
	//	private static IEnumerable<ILinqToTree<T>> DrillDown<T>(this IEnumerable<ILinqToTree<T>> items, Func<ILinqToTree<T>, IEnumerable<ILinqToTree<T>>> function)
	//	{
	//		foreach (var item in items)
	//		{
	//			foreach (ILinqToTree<T> itemChild in function(item))
	//			{
	//				yield return itemChild;
	//			}
	//		}
	//	}

	//	/// <summary>
	//	/// Returns a collection of descendant elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> Descendants<T>(this IEnumerable<ILinqToTree<T>> items)
	//	{
	//		return items.DrillDown(i => i.Descendants());
	//	}

	//	/// <summary>
	//	/// Returns a collection containing this element and all descendant elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> DescendantsAndSelf<T>(this IEnumerable<ILinqToTree<T>> items)
	//	{
	//		return items.DrillDown(i => i.DescendantsAndSelf());
	//	}

	//	/// <summary>
	//	/// Returns a collection of ancestor elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> Ancestors<T>(this IEnumerable<ILinqToTree<T>> items)
	//	{
	//		return items.DrillDown(i => i.Ancestors());
	//	}

	//	/// <summary>
	//	/// Returns a collection containing this element and all ancestor elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> AncestorsAndSelf<T>(this IEnumerable<ILinqToTree<T>> items)
	//	{
	//		return items.DrillDown(i => i.AncestorsAndSelf());
	//	}

	//	/// <summary>
	//	/// Returns a collection of child elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> Elements<T>(this IEnumerable<ILinqToTree<T>> items)
	//	{
	//		return items.DrillDown(i => i.Elements());
	//	}

	//	/// <summary>
	//	/// Returns a collection containing this element and all child elements.
	//	/// </summary>
	//	public static IEnumerable<ILinqToTree<T>> ElementsAndSelf<T>(this IEnumerable<ILinqToTree<T>> items)
	//	{
	//		return items.DrillDown(i => i.ElementsAndSelf());
	//	}

	//	#endregion Enumerable extensions
	//}
}