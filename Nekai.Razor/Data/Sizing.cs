namespace Nekai.Razor;

public enum Sizing
{
	Default,
	Small,
	Large
}

public static class ButtonSizeExtensions
{
	public static string AsButtonClass(this Sizing size)
	{
		return size switch
		{
			Sizing.Large => "btn-lg",
			Sizing.Small => "btn-sm",
			_ => ""
		};
	}

	public static string AsSelectClass(this Sizing size)
		=> size switch
		{
			Sizing.Large => "form-select-lg",
			Sizing.Small => "form-select-sm",
			_ => ""
		};

	public static string AsControlClass(this Sizing size)
		=> size switch
		{
			Sizing.Large => "form-control-lg",
			Sizing.Small => "form-control-sm",
			_ => ""
		};

	public static string AsSpinnerClass(this Sizing size)
		=> size switch
		{
			Sizing.Large => "spinner-border-lg",
			Sizing.Small => "spinner-border-sm",
			_ => ""
		};

	public static string AsPulsatingCircleClass(this Sizing size)
		=> size switch
		{
			Sizing.Large => "spinner-grow-lg",
			Sizing.Small => "spinner-grow-sm",
			_ => ""
		};
	
	public static string AsSkeletonSize(this Sizing size)
	=> size switch
	{
		Sizing.Large => "placeholder-lg",
		Sizing.Small => "placeholder-sm",
		_ => ""
	};
}