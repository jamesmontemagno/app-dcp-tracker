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

        // Services
        builder.Services.AddSingleton<IDeferralBucketRepository, SqliteDeferralBucketRepository>();
        builder.Services.AddSingleton<IDistributionProjectionService, DistributionProjectionService>();
        builder.Services.AddSingleton<IGoalsService, GoalsService>();
        builder.Services.AddSingleton<IThemePreferenceService, ThemePreferenceService>();

        // Shell
        builder.Services.AddSingleton<AppShell>();

        // ViewModels
        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddSingleton<BucketsViewModel>();
        builder.Services.AddSingleton<ProjectionsViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();

        // Views
        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddSingleton<BucketsPage>();
        builder.Services.AddSingleton<ProjectionsPage>();
        builder.Services.AddSingleton<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
