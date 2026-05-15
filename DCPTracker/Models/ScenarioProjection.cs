namespace DCPTracker.Models;

public sealed class ScenarioProjection
{
	public required string Name { get; init; }

	public required string AssumptionSummary { get; init; }

	public required decimal ProjectedBalance { get; init; }

	public required decimal EstimatedAnnualIncome { get; init; }

	public required decimal EstimatedTaxWithholding { get; init; }

	public required string Narrative { get; init; }
}