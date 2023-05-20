using Nekai.Common;

namespace Nekai.Blazor;

public abstract class NekaiStyleSet<TStyle> : ConfigurationFileManager<NekaiStyleSet<TStyle>>
	where TStyle : INekaiStyle
{	
	public abstract TStyle MainContentStyle { get; }
	public abstract TStyle ContentStyle { get; }
	public abstract TStyle SubContentStyle { get; }

	public NekaiStyleSet(string baseFilepath)
		: base(baseFilepath)
	{
		Result result = NekaiPath.IsValidPath(baseFilepath);
		if(!result.IsSuccess)
		{
			Exceptor.ThrowAndLogIfDebug(new ArgumentException(result.Message, nameof(baseFilepath)));
			return;
		}

		if(!File.Exists(baseFilepath))
			return;		// Nothing to load - keep the default values
	}
}
