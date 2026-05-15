using DCPTracker.Data;
using DCPTracker.Models;
using SQLite;

namespace DCPTracker.Services;

public sealed class SqliteAccountRepository : IAccountRepository
{
	private readonly SemaphoreSlim initializationGate = new(1, 1);
	private SQLiteAsyncConnection? database;

	public async Task<IReadOnlyList<RetirementAccount>> GetAccountsAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var connection = await GetDatabaseAsync(cancellationToken);
		var records = await connection.Table<AccountRecord>()
			.OrderByDescending(record => record.Balance)
			.ToListAsync();

		return [.. records.Select(record => record.ToDomain())];
	}

	public async Task SaveAccountAsync(RetirementAccount account, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var connection = await GetDatabaseAsync(cancellationToken);
		await connection.InsertOrReplaceAsync(AccountRecord.FromDomain(account));
	}

	public async Task DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var connection = await GetDatabaseAsync(cancellationToken);
		await connection.DeleteAsync<AccountRecord>(accountId.ToString("D"));
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
			await connection.CreateTableAsync<AccountRecord>();
			await SeedStarterAccountsAsync(connection);
			database = connection;
			return database;
		}
		finally
		{
			initializationGate.Release();
		}
	}

	private static async Task SeedStarterAccountsAsync(SQLiteAsyncConnection connection)
	{
		if (await connection.Table<AccountRecord>().CountAsync() > 0)
		{
			return;
		}

		var now = DateTimeOffset.UtcNow;
		var starterAccounts = new[]
		{
			CreateSeedAccount("Workplace retirement", AccountType.Retirement401K, 186_450m, now),
			CreateSeedAccount("Deferred compensation", AccountType.DeferredCompensation, 74_000m, now),
			CreateSeedAccount("Unvested equity snapshot", AccountType.RestrictedStockUnits, 41_500m, now)
		};

		await connection.InsertAllAsync(starterAccounts.Select(AccountRecord.FromDomain));
	}

	private static RetirementAccount CreateSeedAccount(string name, AccountType accountType, decimal balance, DateTimeOffset updatedAt) => new()
	{
		Id = Guid.NewGuid(),
		Name = name,
		AccountType = accountType,
		Balance = balance,
		UpdatedAt = updatedAt
	};
}