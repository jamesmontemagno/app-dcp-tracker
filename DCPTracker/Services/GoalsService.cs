using DCPTracker.Models;

namespace DCPTracker.Services;

public sealed class GoalsService : IGoalsService
{
	public AppGoals Load() => new()
	{
		MonthlyDistributionGoal = Convert.ToDecimal(
			Preferences.Default.Get(nameof(AppGoals.MonthlyDistributionGoal), 5_000d)),
		DefaultExpectedAnnualReturnRate = Convert.ToDecimal(
			Preferences.Default.Get(nameof(AppGoals.DefaultExpectedAnnualReturnRate), 0.07d)),
		TaxWithholdingRate = Convert.ToDecimal(
			Preferences.Default.Get(nameof(AppGoals.TaxWithholdingRate), 0.24d))
	};

	public void Save(AppGoals goals)
	{
		Preferences.Default.Set(nameof(AppGoals.MonthlyDistributionGoal),
			Convert.ToDouble(goals.MonthlyDistributionGoal));
		Preferences.Default.Set(nameof(AppGoals.DefaultExpectedAnnualReturnRate),
			Convert.ToDouble(goals.DefaultExpectedAnnualReturnRate));
		Preferences.Default.Set(nameof(AppGoals.TaxWithholdingRate),
			Convert.ToDouble(goals.TaxWithholdingRate));
	}
}
