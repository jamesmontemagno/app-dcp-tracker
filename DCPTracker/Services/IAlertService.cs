using DCPTracker.Models;

namespace DCPTracker.Services;

public interface IAlertService
{
	IReadOnlyList<DashboardAlert> BuildAlerts(IReadOnlyList<RetirementAccount> accounts, IReadOnlyList<PlanningEvent> events, DashboardPreferences preferences);
}