using DCPTracker.Models;

namespace DCPTracker.Services;

public interface IGoalsService
{
	AppGoals Load();

	void Save(AppGoals goals);
}
