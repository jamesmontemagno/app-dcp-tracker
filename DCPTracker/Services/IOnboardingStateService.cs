namespace DCPTracker.Services;

public interface IOnboardingStateService
{
	bool HasCompletedOnboarding { get; }

	void MarkCompleted();
}