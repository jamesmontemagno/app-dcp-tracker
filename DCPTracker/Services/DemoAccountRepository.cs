using DCPTracker.Models;

namespace DCPTracker.Services;

public sealed class DemoAccountRepository : IAccountRepository
{
	private readonly List<RetirementAccount> accounts =
	[
		Create("401(k) diversified portfolio", AccountType.Retirement401K, 248_000m),
		Create("Deferred compensation election", AccountType.DeferredCompensation, 132_000m),
		Create("RSU vest pipeline", AccountType.RestrictedStockUnits, 96_000m),
		Create("ESPP shares", AccountType.EmployeeStockPurchasePlan, 28_500m),
		Create("Brokerage bridge bucket", AccountType.Brokerage, 67_250m)
	];

	public Task<IReadOnlyList<RetirementAccount>> GetAccountsAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		return Task.FromResult<IReadOnlyList<RetirementAccount>>([.. accounts.OrderByDescending(account => account.Balance)]);
	}

	public Task SaveAccountAsync(RetirementAccount account, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var existingIndex = accounts.FindIndex(existingAccount => existingAccount.Id == account.Id);
		if (existingIndex >= 0) accounts[existingIndex] = account; else accounts.Add(account);
		return Task.CompletedTask;
	}

	public Task DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		accounts.RemoveAll(account => account.Id == accountId);
		return Task.CompletedTask;
	}

	private static RetirementAccount Create(string name, AccountType accountType, decimal balance) => new() { Id = Guid.NewGuid(), Name = name, AccountType = accountType, Balance = balance, UpdatedAt = DateTimeOffset.UtcNow };
}