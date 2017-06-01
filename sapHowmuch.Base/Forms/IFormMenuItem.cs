using System.Collections.Generic;

namespace sapHowmuch.Base.Forms
{
	public interface IFormMenuItem
	{
		IEnumerable<FormMenuItem> MenuItems { get; }
	}

	public class FormMenuItem
	{
		public string MenuItemId { get; set; }
		public string MenuItemTitle { get; set; }
		public string ParentMenuItemId { get; set; }
		public int MenuItemPosition { get; set; }
	}
}