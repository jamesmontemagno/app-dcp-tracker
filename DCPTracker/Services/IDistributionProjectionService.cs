using DCPTracker.Models;

namespace DCPTracker.Services;

public interface IDistributionProjectionService
{
	/// <summary>Returns the projected balance of <paramref name="bucket"/> as of <paramref name="asOfDate"/>.</summary>
	decimal GetProjectedBalance(DeferralBucket bucket, DateTime asOfDate, decimal defaultAnnualReturnRate);

	/// <summary>Returns the estimated fixed monthly distribution amount for <paramref name="bucket"/>.</summary>
	decimal GetEstimatedMonthlyDistribution(DeferralBucket bucket, decimal defaultAnnualReturnRate);

	/// <summary>Returns the total distribution from all active buckets for the given calendar month.</summary>
	decimal GetTotalMonthlyDistributionAt(IReadOnlyList<DeferralBucket> buckets, DateTime month, decimal defaultAnnualReturnRate);

	/// <summary>Returns weighted average expected annual return rate across all buckets.</summary>
	decimal GetWeightedAverageReturnRate(IReadOnlyList<DeferralBucket> buckets, decimal defaultAnnualReturnRate);

	/// <summary>Builds the full month-by-month distribution timeline for all buckets.</summary>
	IReadOnlyList<MonthlyDistributionRow> BuildDistributionTimeline(IReadOnlyList<DeferralBucket> buckets, AppGoals goals);
}
