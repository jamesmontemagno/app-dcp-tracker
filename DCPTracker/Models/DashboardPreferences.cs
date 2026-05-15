namespace DCPTracker.Models;

public sealed class DashboardPreferences
{
	public int RetirementAge { get; init; } = 60;

	public decimal AnnualContribution { get; init; } = 30_000m;

	public decimal ExpectedAnnualGrowthRate { get; init; } = 0.05m;

	public decimal ConcentrationWarningThreshold { get; init; } = 0.25m;

	public decimal ConcentrationCriticalThreshold { get; init; } = 0.40m;

	public decimal TaxWithholdingRate { get; init; } = 0.24m;

	public string TaxBracketLabel { get; init; } = "24%";

	public bool ShowAlerts { get; init; } = true;

	public bool ShowTimeline { get; init; } = true;

	public bool ShowScenarios { get; init; } = true;
}