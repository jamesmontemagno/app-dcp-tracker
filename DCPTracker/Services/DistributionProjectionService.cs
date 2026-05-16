using DCPTracker.Models;

namespace DCPTracker.Services;

public sealed class DistributionProjectionService : IDistributionProjectionService
{
	public decimal GetProjectedBalance(DeferralBucket bucket, DateTime asOfDate, decimal defaultAnnualReturnRate)
	{
		var annualRate = GetBucketWeightedRate(bucket, defaultAnnualReturnRate);
		// Growth period starts at Jan 1 of the deferral year
		var startDate = new DateTime(bucket.DeferralYear, 1, 1);
		var yearsOfGrowth = Math.Max((asOfDate - startDate).TotalDays / 365.25, 0);
		return bucket.DeferredAmount * (decimal)Math.Pow((double)(1m + annualRate), yearsOfGrowth);
	}

	public decimal GetEstimatedMonthlyDistribution(DeferralBucket bucket, decimal defaultAnnualReturnRate)
	{
		if (bucket.DistributionMonths <= 0)
		{
			return 0m;
		}

		var projectedAtStart = GetProjectedBalance(bucket, bucket.DistributionStartDate, defaultAnnualReturnRate);
		return projectedAtStart / bucket.DistributionMonths;
	}

	public decimal GetTotalMonthlyDistributionAt(IReadOnlyList<DeferralBucket> buckets, DateTime month, decimal defaultAnnualReturnRate)
	{
		return buckets
			.Where(b => IsActiveInMonth(b, month))
			.Sum(b => GetEstimatedMonthlyDistribution(b, defaultAnnualReturnRate));
	}

	public decimal GetWeightedAverageReturnRate(IReadOnlyList<DeferralBucket> buckets, decimal defaultAnnualReturnRate)
	{
		if (buckets.Count == 0)
		{
			return defaultAnnualReturnRate;
		}

		var totalDeferred = buckets.Sum(b => b.DeferredAmount);
		if (totalDeferred == 0m)
		{
			return defaultAnnualReturnRate;
		}

		return buckets.Sum(b => GetBucketWeightedRate(b, defaultAnnualReturnRate) * b.DeferredAmount) / totalDeferred;
	}

	public IReadOnlyList<MonthlyDistributionRow> BuildDistributionTimeline(IReadOnlyList<DeferralBucket> buckets, AppGoals goals)
	{
		if (buckets.Count == 0)
		{
			return [];
		}

		var firstMonth = buckets.Min(b => b.DistributionStartDate);
		firstMonth = new DateTime(firstMonth.Year, firstMonth.Month, 1);

		var lastMonth = buckets.Max(b =>
		{
			var start = new DateTime(b.DistributionStartDate.Year, b.DistributionStartDate.Month, 1);
			return start.AddMonths(b.DistributionMonths - 1);
		});

		var rows = new List<MonthlyDistributionRow>();
		var cumulative = 0m;
		var current = firstMonth;

		while (current <= lastMonth)
		{
			var gross = GetTotalMonthlyDistributionAt(buckets, current, goals.DefaultExpectedAnnualReturnRate);
			if (gross > 0m)
			{
				var net = gross * (1m - goals.TaxWithholdingRate);
				cumulative += net;
				rows.Add(new MonthlyDistributionRow
				{
					Month = current,
					GrossAmount = gross,
					NetAmount = net,
					CumulativeNet = cumulative
				});
			}

			current = current.AddMonths(1);
		}

		return rows;
	}

	private static decimal GetBucketWeightedRate(DeferralBucket bucket, decimal defaultRate)
	{
		if (bucket.Investments.Count == 0)
		{
			return defaultRate;
		}

		var totalAllocation = bucket.Investments.Sum(i => i.AllocationPercent);
		if (totalAllocation == 0m)
		{
			return defaultRate;
		}

		return bucket.Investments.Sum(i => i.ExpectedAnnualReturnRate * i.AllocationPercent) / totalAllocation;
	}

	private static bool IsActiveInMonth(DeferralBucket bucket, DateTime month)
	{
		var startMonth = new DateTime(bucket.DistributionStartDate.Year, bucket.DistributionStartDate.Month, 1);
		var endMonth = startMonth.AddMonths(bucket.DistributionMonths - 1);
		var normalizedMonth = new DateTime(month.Year, month.Month, 1);
		return normalizedMonth >= startMonth && normalizedMonth <= endMonth;
	}
}
