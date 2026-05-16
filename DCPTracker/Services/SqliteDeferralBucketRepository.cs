using DCPTracker.Data;
using DCPTracker.Models;
using SQLite;

namespace DCPTracker.Services;

public sealed class SqliteDeferralBucketRepository : IDeferralBucketRepository
{
	private readonly SemaphoreSlim initializationGate = new(1, 1);
	private SQLiteAsyncConnection? database;

	public async Task<IReadOnlyList<DeferralBucket>> GetBucketsAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var connection = await GetDatabaseAsync(cancellationToken);
		var records = await connection.Table<DeferralBucketRecord>()
			.OrderBy(record => record.DistributionStartDate)
			.ToListAsync();

		return [.. records.Select(record => record.ToDomain())];
	}

	public async Task SaveBucketAsync(DeferralBucket bucket, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var connection = await GetDatabaseAsync(cancellationToken);
		await connection.InsertOrReplaceAsync(DeferralBucketRecord.FromDomain(bucket));
	}

	public async Task DeleteBucketAsync(Guid bucketId, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var connection = await GetDatabaseAsync(cancellationToken);
		await connection.DeleteAsync<DeferralBucketRecord>(bucketId.ToString("D"));
	}

	private async Task<SQLiteAsyncConnection> GetDatabaseAsync(CancellationToken cancellationToken)
	{
		if (database is not null)
		{
			return database;
		}

		await initializationGate.WaitAsync(cancellationToken);
		try
		{
			if (database is not null)
			{
				return database;
			}

			var connection = new SQLiteAsyncConnection(LocalDatabaseOptions.DatabasePath, LocalDatabaseOptions.Flags);
			await connection.CreateTableAsync<DeferralBucketRecord>();
			await SeedStarterBucketsAsync(connection);
			database = connection;
			return database;
		}
		finally
		{
			initializationGate.Release();
		}
	}

	private static async Task SeedStarterBucketsAsync(SQLiteAsyncConnection connection)
	{
		if (await connection.Table<DeferralBucketRecord>().CountAsync() > 0)
		{
			return;
		}

		var today = DateTime.Today;
		var starterBuckets = new[]
		{
			CreateSeedBucket("2022 Deferral", 2022, 42_000m, today.AddYears(3), 60,
				[new InvestmentAllocation { FundName = "S&P 500 Index", AllocationPercent = 70m, ExpectedAnnualReturnRate = 0.08m },
				 new InvestmentAllocation { FundName = "Bond Index", AllocationPercent = 30m, ExpectedAnnualReturnRate = 0.04m }]),
			CreateSeedBucket("2023 Deferral", 2023, 55_000m, today.AddYears(5), 120,
				[new InvestmentAllocation { FundName = "S&P 500 Index", AllocationPercent = 80m, ExpectedAnnualReturnRate = 0.08m },
				 new InvestmentAllocation { FundName = "International Index", AllocationPercent = 20m, ExpectedAnnualReturnRate = 0.06m }])
		};

		await connection.InsertAllAsync(starterBuckets.Select(DeferralBucketRecord.FromDomain));
	}

	private static DeferralBucket CreateSeedBucket(
		string label,
		int deferralYear,
		decimal deferredAmount,
		DateTime distributionStartDate,
		int distributionMonths,
		IReadOnlyList<InvestmentAllocation> investments) => new()
	{
		Id = Guid.NewGuid(),
		Label = label,
		DeferralYear = deferralYear,
		DeferredAmount = deferredAmount,
		DistributionStartDate = distributionStartDate,
		DistributionMonths = distributionMonths,
		Investments = investments
	};
}
