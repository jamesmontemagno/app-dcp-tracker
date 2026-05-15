using DCPTracker.Models;

namespace DCPTracker.Services;

public interface IAccountRepository
{
	Task<IReadOnlyList<RetirementAccount>> GetAccountsAsync(CancellationToken cancellationToken = default);

	Task SaveAccountAsync(RetirementAccount account, CancellationToken cancellationToken = default);

	Task DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
}