using DCPTracker.Services;

namespace DCPTracker;

public partial class App : Application
{
	private readonly AppShell appShell;

	public App(AppShell appShell, IThemePreferenceService themePreferenceService)
	{
		InitializeComponent();
		this.appShell = appShell;
		themePreferenceService.ApplySavedTheme();
	}

	protected override Window CreateWindow(IActivationState? activationState)
		=> new(appShell);
}