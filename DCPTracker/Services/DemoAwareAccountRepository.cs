using DCPTracker.Models;

namespace DCPTracker.Services;

public sealed class DemoAwareAccountRepository : IAccountRepository
{
	private readonly SqliteAccountRepository sqliteRepository;
	private readonly DemoAccountRepository demoRepository;
	private readonly IDemoModeService demoModeService;

	public DemoAwareAccountRepository(SqliteAccountRepository sqliteRepository, DemoAccountRepository demoRepository, IDemoModeService demoModeService)
	{
		this.sqliteRepository = sqliteRepository;
		this.demoRepository = demoRepository;
		this.demoModeService = demoModeService;
	}

	public Task<IReadOnlyList<RetirementAccount>> GetAccountsAsync(CancellationToken cancellationToken = default) => ActiveRepository.GetAccountsAsync(cancellationToken);
	public Task SaveAccountAsync(RetirementAccount account, CancellationToken cancellationToken = default) => ActiveRepository.SaveAccountAsync(account, cancellationToken);
	public Task DeleteAccountAsync(Guid accountId, CancellationToken cancellationToken = default) => ActiveRepository.DeleteAccountAsync(accountId, cancellationToken);
	private IAccountRepository ActiveRepository => demoModeService.IsDemoModeEnabled ? demoRepository : sqliteRepository;
}