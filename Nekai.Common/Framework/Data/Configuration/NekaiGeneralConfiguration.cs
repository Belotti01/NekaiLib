using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Nekai.Common;

/// <summary>
/// Singleton implementation of a <see cref="JsonSerializableObject{TSelf}"/> that handles the general configuration shared among
/// all Nekai applications.
/// </summary>
[JsonSerializable(typeof(NekaiGeneralConfiguration))]
public partial class NekaiGeneralConfiguration : JsonSerializableObject<NekaiGeneralConfiguration>
{
	/// <summary>
	/// Unique instance of this class.
	/// </summary>
	[JsonIgnore]
	public static NekaiGeneralConfiguration Singleton => _instanceInitializer.Value;

	[JsonIgnore]
	private static readonly Lazy<NekaiGeneralConfiguration> _instanceInitializer = new(_CreateSingleton);

	/// <summary> The default display language. </summary>
	public DisplayLanguage DefaultLanguage { get; set; } = Common.DisplayLanguage.EnglishUsa;
	/// <summary> Whether to use dark color tones for the UI when possible. </summary>
	public bool PreferDarkMode { get; set; } = true;

	public NekaiGeneralConfiguration(PathString? filePath = null)
		: base(filePath)
	{
		if(DefaultLanguage == Common.DisplayLanguage.Default)
		{
			// Fallback to the OS language if none is set in the configuration.
			TryLoadOSLanguage();
		}
	}

	private static NekaiGeneralConfiguration _CreateSingleton()
	{
		var singletonFilePath = NekaiData.Files.GeneralSettingsFile;
		
		if(singletonFilePath.CanBeReadAsFile())
		{
			// File is accessible, deserialize it.
			var deserializationResult = TryDeserialize(singletonFilePath);
			if(!deserializationResult.IsSuccessful)
			{
				NekaiLogs.Shared.Error("General configuration file exists, but deserialization failed. Loading default values...");
				return new();
			}

			var config = deserializationResult.Value;
			return deserializationResult.Value;
		}

		// If the file is not found or something goes wrong, load the default values instead.
		NekaiGeneralConfiguration result = new(singletonFilePath);
		return result;
	}

	/// <summary>
	/// Attempt to set the <see cref="DefaultLanguage"/> to the operating system's UI language.
	/// </summary>
	/// <returns>
	/// <see langword="true"/> if the OS language was loaded, <see langword="false"/> if the operation failed or the OS's language is
	/// not supported.
	/// </returns>
	public bool TryLoadOSLanguage()
	{
		if(_TryGetOSLanguage(out DisplayLanguage osLanguage))
		{
			DefaultLanguage = osLanguage;
			return true;
		}
		return false;
	}

	private static bool _TryGetOSLanguage(out DisplayLanguage osLanguage)
	{
		try
		{
			string isoOSLanguage = CultureInfo.InstalledUICulture.ThreeLetterISOLanguageName;
			foreach(DisplayLanguage language in Enum.GetValues<DisplayLanguage>())
			{
				string isoLanguage = language.ToThreeLetterISOName();
				if(isoOSLanguage.EqualsIgnoreCase(isoLanguage))
				{
					osLanguage = language;
					return true;
				}
			}
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Error("Couldn't load default system language: {ex}", ex.Message);
			Debug.Fail($"Default language could not be loaded.", ex.Message);
		}

		osLanguage = Common.DisplayLanguage.Default;    // Use a default language to fall back to.
		return false;
	}
}