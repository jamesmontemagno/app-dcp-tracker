using DCPTracker.Models;

namespace DCPTracker.Services;

public interface IScenarioProjectionService
{
	IReadOnlyList<ScenarioProjection> BuildScenarios(IReadOnlyList<RetirementAccount> accounts, DashboardPreferences preferences);
}