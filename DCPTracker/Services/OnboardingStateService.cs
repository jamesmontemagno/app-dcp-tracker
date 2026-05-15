namespace DCPTracker.Services;

public sealed class OnboardingStateService : IOnboardingStateService
{
	private const string OnboardingCompleteKey = "OnboardingComplete";

	public bool HasCompletedOnboarding => Preferences.Default.Get(OnboardingCompleteKey, false);

	public void MarkCompleted() => Preferences.Default.Set(OnboardingCompleteKey, true);
}