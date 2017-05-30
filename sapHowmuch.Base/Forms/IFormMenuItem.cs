namespace sapHowmuch.Base.Forms
{
	public interface IFormMenuItem
	{
		string MenuItemId { get; }
		string MenuItemTitle { get; }
		string ParentMenuItemId { get; }
		int MenuItemPosition { get; }
	}
}