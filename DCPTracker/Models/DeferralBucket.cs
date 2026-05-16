namespace DCPTracker.Models;

public sealed class DeferralBucket
{
	public required Guid Id { get; init; }

	/// <summary>Human-readable label, e.g. "2022 Deferral".</summary>
	public required string Label { get; init; }

	/// <summary>Calendar year the compensation was deferred.</summary>
	public required int DeferralYear { get; init; }

	/// <summary>Original amount deferred before any growth.</summary>
	public required decimal DeferredAmount { get; init; }

	/// <summary>Date on which distribution payments begin.</summary>
	public required DateTime DistributionStartDate { get; init; }

	/// <summary>Number of months over which distributions are paid.</summary>
	public required int DistributionMonths { get; init; }

	/// <summary>Investment allocations within this bucket.</summary>
	public required IReadOnlyList<InvestmentAllocation> Investments { get; init; }

	public string DistributionStartLabel => DistributionStartDate.ToString("MMM yyyy");
}
