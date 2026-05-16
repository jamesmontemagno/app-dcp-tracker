using DCPTracker.Models;
using SQLite;
using System.Text.Json;

namespace DCPTracker.Data;

[Table("DeferralBuckets")]
public sealed class DeferralBucketRecord
{
	[PrimaryKey]
	public string Id { get; set; } = string.Empty;

	public string Label { get; set; } = string.Empty;

	public int DeferralYear { get; set; }

	public decimal DeferredAmount { get; set; }

	public DateTime DistributionStartDate { get; set; }

	public int DistributionMonths { get; set; }

	/// <summary>JSON-serialized list of InvestmentAllocationRecord.</summary>
	public string InvestmentsJson { get; set; } = "[]";

	public static DeferralBucketRecord FromDomain(DeferralBucket bucket)
	{
		var allocationRecords = bucket.Investments
			.Select(inv => new InvestmentAllocationRecord
			{
				FundName = inv.FundName,
				AllocationPercent = inv.AllocationPercent,
				ExpectedAnnualReturnRate = inv.ExpectedAnnualReturnRate
			})
			.ToList();

		return new DeferralBucketRecord
		{
			Id = bucket.Id.ToString("D"),
			Label = bucket.Label,
			DeferralYear = bucket.DeferralYear,
			DeferredAmount = bucket.DeferredAmount,
			DistributionStartDate = bucket.DistributionStartDate,
			DistributionMonths = bucket.DistributionMonths,
			InvestmentsJson = JsonSerializer.Serialize(allocationRecords)
		};
	}

	public DeferralBucket ToDomain()
	{
		var allocationRecords = JsonSerializer.Deserialize<List<InvestmentAllocationRecord>>(InvestmentsJson)
			?? [];

		return new DeferralBucket
		{
			Id = Guid.Parse(Id),
			Label = Label,
			DeferralYear = DeferralYear,
			DeferredAmount = DeferredAmount,
			DistributionStartDate = DistributionStartDate,
			DistributionMonths = DistributionMonths,
			Investments = [.. allocationRecords.Select(r => new InvestmentAllocation
			{
				FundName = r.FundName,
				AllocationPercent = r.AllocationPercent,
				ExpectedAnnualReturnRate = r.ExpectedAnnualReturnRate
			})]
		};
	}

	private sealed class InvestmentAllocationRecord
	{
		public string FundName { get; set; } = string.Empty;
		public decimal AllocationPercent { get; set; }
		public decimal ExpectedAnnualReturnRate { get; set; }
	}
}
