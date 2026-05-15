namespace DCPTracker.Services;

public interface IDemoModeService
{
	bool IsDemoModeEnabled { get; }

	void SetDemoMode(bool isEnabled);
}