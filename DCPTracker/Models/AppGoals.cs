namespace DCPTracker.Models;

public sealed class AppGoals
{
	/// <summary>Target monthly net distribution amount.</summary>
	public decimal MonthlyDistributionGoal { get; init; } = 5_000m;

	/// <summary>Fallback annual return rate (decimal) when no fund-level rate is set.</summary>
	public decimal DefaultExpectedAnnualReturnRate { get; init; } = 0.07m;

	/// <summary>Tax withholding rate applied to gross distributions (decimal).</summary>
	public decimal TaxWithholdingRate { get; init; } = 0.24m;
}
