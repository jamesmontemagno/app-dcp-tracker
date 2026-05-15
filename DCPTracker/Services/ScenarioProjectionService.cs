using DCPTracker.Models;

namespace DCPTracker.Services;

public sealed class ScenarioProjectionService : IScenarioProjectionService
{
	private const int PlanningBaselineAge = 40;

	public IReadOnlyList<ScenarioProjection> BuildScenarios(IReadOnlyList<RetirementAccount> accounts, DashboardPreferences preferences)
	{
		var balance = accounts.Sum(account => account.Balance);
		var yearsToRetirement = Math.Max(preferences.RetirementAge - PlanningBaselineAge, 1);

		return
		[
			Create("Target retirement", $"Retire at {preferences.RetirementAge}", balance, preferences.AnnualContribution, preferences.ExpectedAnnualGrowthRate, yearsToRetirement, preferences, "Uses your saved retirement age and annual contribution inputs."),
			Create("Leave Microsoft in 2 years", "Contributions modeled for two more years", balance, preferences.AnnualContribution, preferences.ExpectedAnnualGrowthRate, 2, preferences, "Useful for comparing a near-term departure against the full target runway."),
			Create("Market drawdown", "Immediate 20% decline, then baseline growth", balance * 0.80m, preferences.AnnualContribution, preferences.ExpectedAnnualGrowthRate, yearsToRetirement, preferences, "Stress test only; it is deterministic and not a probability forecast."),
			Create("Max-contribution lens", "Annual savings increased by 20%", balance, preferences.AnnualContribution * 1.20m, preferences.ExpectedAnnualGrowthRate, yearsToRetirement, preferences, "Shows sensitivity to a stronger contribution assumption, not a recommendation.")
		];
	}

	private static ScenarioProjection Create(string name, string assumptionSummary, decimal openingBalance, decimal annualContribution, decimal growthRate, int years, DashboardPreferences preferences, string narrative)
	{
		var projectedBalance = openingBalance;
		for (var year = 0; year < years; year++)
		{
			projectedBalance = (projectedBalance + annualContribution) * (1m + growthRate);
		}

		var estimatedAnnualIncome = projectedBalance * 0.04m;
		return new ScenarioProjection
		{
			Name = name,
			AssumptionSummary = assumptionSummary,
			ProjectedBalance = projectedBalance,
			EstimatedAnnualIncome = estimatedAnnualIncome,
			EstimatedTaxWithholding = estimatedAnnualIncome * preferences.TaxWithholdingRate,
			Narrative = narrative
		};
	}
}