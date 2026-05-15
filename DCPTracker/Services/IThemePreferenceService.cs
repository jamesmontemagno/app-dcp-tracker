namespace DCPTracker.Services;

public interface IThemePreferenceService
{
	string CurrentTheme { get; }

	IReadOnlyList<string> ThemeOptions { get; }

	void ApplySavedTheme();

	void SaveAndApply(string themeName);
}