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
public class NekaiGeneralConfiguration : JsonSerializableObject<NekaiGeneralConfiguration>
{
	/// <summary> The file containing the serialized configuration. </summary>
	private static string _FilePath => NekaiData.Files.GeneralSettingsFile;

	/// <summary>
	/// Unique instance of this class.
	/// </summary>
	public static NekaiGeneralConfiguration Singleton => _instanceInitializer.Value;

	private static readonly Lazy<NekaiGeneralConfiguration> _instanceInitializer = new(_CreateSingleton);

	/// <summary> The default display language. </summary>
	public DisplayLanguage? DefaultLanguage { get; set; }

	/// <summary> Whether to use dark color tones for the UI when possible. </summary>
	public bool PreferDarkMode { get; set; } = true;

	public NekaiGeneralConfiguration()
		: base(_FilePath)
	{
		if(DefaultLanguage is null)
		{
			// Fallback to the OS language if none is set in the configuration.
			TryLoadOSLanguage();
		}
	}

	private static NekaiGeneralConfiguration _CreateSingleton()
	{
		var result = PathString.TryParse(_FilePath);
		if(!result.IsSuccessful)
			Exceptor.ThrowCritical(AppExitCode.FixedPathError, result.Error.GetMessage());
		
		var path = result.Value;
		if(path.CanBeReadAsFile())
		{
			// File is accessible, deserialize it.
			var deserializationResult = TryDeserialize(_FilePath);
			if(deserializationResult.IsSuccessful)
				return deserializationResult.Value;

			NekaiLogs.Shared.Error("General configuration file exists, but deserialization failed. Creating a new instance...");
		}

		// If file is not found or something goes wrong, load the default values instead.
		return new();
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
			Debug.Fail($"Default language could not be loaded.", ex.Message);
		}

		osLanguage = DisplayLanguage.EnglishUsa;    // Use a default language to fall back to.
		return false;
	}
}