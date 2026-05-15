namespace DCPTracker.Services;

public sealed class DemoModeService : IDemoModeService
{
	private const string DemoModePreferenceKey = "DemoModeEnabled";

	public bool IsDemoModeEnabled => Preferences.Default.Get(DemoModePreferenceKey, false);

	public void SetDemoMode(bool isEnabled) => Preferences.Default.Set(DemoModePreferenceKey, isEnabled);
}