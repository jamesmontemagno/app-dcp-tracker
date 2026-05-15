using CommunityToolkit.Maui;
using DCPTracker.Services;
using DCPTracker.ViewModels;
using DCPTracker.Views;
using Microsoft.Extensions.Logging;

namespace DCPTracker;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IDemoModeService, DemoModeService>();
		builder.Services.AddSingleton<SqliteAccountRepository>();
		builder.Services.AddSingleton<DemoAccountRepository>();
		builder.Services.AddSingleton<IAccountRepository, DemoAwareAccountRepository>();
		builder.Services.AddSingleton<SqlitePlanningEventRepository>();
		builder.Services.AddSingleton<DemoPlanningEventRepository>();
		builder.Services.AddSingleton<IPlanningEventRepository, DemoAwarePlanningEventRepository>();
		builder.Services.AddSingleton<IDashboardPreferencesService, DashboardPreferencesService>();
		builder.Services.AddSingleton<IAccountImportService, AccountImportService>();
		builder.Services.AddSingleton<IAlertService, AlertService>();
		builder.Services.AddSingleton<IScenarioProjectionService, ScenarioProjectionService>();
		builder.Services.AddSingleton<IThemePreferenceService, ThemePreferenceService>();
		builder.Services.AddSingleton<IOnboardingStateService, OnboardingStateService>();
		builder.Services.AddSingleton<AppShell>();
		builder.Services.AddSingleton<OnboardingViewModel>();
		builder.Services.AddSingleton<OnboardingPage>();
		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddSingleton<DashboardPage>();
		builder.Services.AddSingleton<DataPage>();
		builder.Services.AddSingleton<GoalsPage>();
		builder.Services.AddSingleton<TimelinePage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
