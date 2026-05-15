using DCPTracker.Services;

namespace DCPTracker;

public partial class AppShell : Shell
{
	public AppShell(IOnboardingStateService onboardingStateService)
	{
		InitializeComponent();
		CurrentItem = onboardingStateService.HasCompletedOnboarding
			? DashboardPageContent
			: OnboardingPageContent;
	}
}
