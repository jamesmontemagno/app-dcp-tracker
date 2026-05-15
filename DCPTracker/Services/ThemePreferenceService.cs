namespace DCPTracker.Services;

public sealed class ThemePreferenceService : IThemePreferenceService
{
	private const string ThemePreferenceKey = "AppTheme";

	public string CurrentTheme => Preferences.Default.Get(ThemePreferenceKey, "System");

	public IReadOnlyList<string> ThemeOptions { get; } = ["System", "Light", "Dark"];

	public void ApplySavedTheme() => ApplyTheme(CurrentTheme);

	public void SaveAndApply(string themeName)
	{
		var resolvedTheme = ThemeOptions.Contains(themeName) ? themeName : "System";
		Preferences.Default.Set(ThemePreferenceKey, resolvedTheme);
		ApplyTheme(resolvedTheme);
	}

	private static void ApplyTheme(string themeName)
	{
		if (Application.Current is null)
		{
			return;
		}

		Application.Current.UserAppTheme = themeName switch
		{
			"Light" => AppTheme.Light,
			"Dark" => AppTheme.Dark,
			_ => AppTheme.Unspecified
		};
	}
}