namespace DCPTracker.Models;

public sealed class InvestmentAllocation
{
	public required string FundName { get; init; }

	/// <summary>Percentage of bucket allocated to this fund (0–100).</summary>
	public required decimal AllocationPercent { get; init; }

	/// <summary>Expected annual return rate as a decimal, e.g. 0.07 for 7%.</summary>
	public required decimal ExpectedAnnualReturnRate { get; init; }
}
