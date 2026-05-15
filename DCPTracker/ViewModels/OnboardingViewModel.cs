using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DCPTracker.Services;

namespace DCPTracker.ViewModels;

public partial class OnboardingViewModel : ObservableObject
{
	private readonly IOnboardingStateService onboardingStateService;
	private readonly IThemePreferenceService themePreferenceService;
	private readonly IDemoModeService demoModeService;

	public OnboardingViewModel(IOnboardingStateService onboardingStateService, IThemePreferenceService themePreferenceService, IDemoModeService demoModeService)
	{
		this.onboardingStateService = onboardingStateService;
		this.themePreferenceService = themePreferenceService;
		this.demoModeService = demoModeService;
		SelectedTheme = themePreferenceService.CurrentTheme;
		IsDemoModeEnabled = demoModeService.IsDemoModeEnabled;
	}

	public IReadOnlyList<string> ThemeOptions => themePreferenceService.ThemeOptions;

	[ObservableProperty]
	private string selectedTheme = "System";

	[ObservableProperty]
	private bool isDemoModeEnabled;

	partial void OnSelectedThemeChanged(string value) => themePreferenceService.SaveAndApply(value);

	partial void OnIsDemoModeEnabledChanged(bool value) => demoModeService.SetDemoMode(value);

	[RelayCommand]
	private async Task CompleteAsync()
	{
		onboardingStateService.MarkCompleted();
		await Shell.Current.GoToAsync("//DashboardPage");
	}
}