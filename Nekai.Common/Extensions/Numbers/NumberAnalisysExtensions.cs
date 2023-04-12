namespace Nekai.Common;

public static class NumberAnalisysExtensions
{
	public static int CountDigits(this int number)
	{
		if(number == 0)
			return 1;
		return (int)Math.Floor(Math.Log10(number) + 1);
	}
}