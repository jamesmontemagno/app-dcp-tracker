using DCPTracker.Models;

namespace DCPTracker.Services;

public sealed class AlertService : IAlertService
{
	public IReadOnlyList<DashboardAlert> BuildAlerts(IReadOnlyList<RetirementAccount> accounts, IReadOnlyList<PlanningEvent> events, DashboardPreferences preferences)
	{
		var alerts = new List<DashboardAlert>();
		var totalBalance = accounts.Sum(account => account.Balance);
		var employerEquityBalance = accounts
			.Where(account => account.AccountType is AccountType.RestrictedStockUnits or AccountType.EmployeeStockPurchasePlan)
			.Sum(account => account.Balance);
		var concentration = totalBalance == 0m ? 0m : employerEquityBalance / totalBalance;

		if (concentration >= preferences.ConcentrationCriticalThreshold)
		{
			alerts.Add(Create("High", "Employer stock concentration is above your critical threshold.", $"Current modeled concentration: {concentration:P0}."));
		}
		else if (concentration >= preferences.ConcentrationWarningThreshold)
		{
			alerts.Add(Create("Watch", "Employer stock concentration is above your watch threshold.", $"Current modeled concentration: {concentration:P0}."));
		}

		var upcomingEvent = events.FirstOrDefault(planningEvent => planningEvent.EventDate >= DateTime.Today && planningEvent.EventDate <= DateTime.Today.AddDays(90));
		if (upcomingEvent is not null)
		{
			alerts.Add(Create("Upcoming", upcomingEvent.Title, $"{upcomingEvent.EventType} event on {upcomingEvent.EventDateLabel}."));
		}

		if (accounts.Any(account => account.AccountType == AccountType.DeferredCompensation))
		{
			alerts.Add(Create("Info", "DCP values are tracked as planning inputs.", "Confirm elections and distribution rules in official plan materials before acting."));
		}

		if (preferences.AnnualContribution <= 0m)
		{
			alerts.Add(Create("Watch", "Annual contribution assumption is zero.", "Scenario projections will model balance growth without new contributions."));
		}

		return alerts;
	}

	private static DashboardAlert Create(string severity, string title, string detail) => new()
	{
		Severity = severity,
		Title = title,
		Detail = detail
	};
}