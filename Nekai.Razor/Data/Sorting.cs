namespace Nekai.Razor;

public enum Sorting
{
	None,
	Ascending,
	Descending
}

public static class SortingExtensions
{
	public static string ToIcon(this Sorting sortOrder)
	{
		return sortOrder switch
		{
			Sorting.Ascending => "keyboard_arrow_down",
			Sorting.Descending => "keyboard_arrow_up",
			_ => "swap_vert"
		};
	}
}