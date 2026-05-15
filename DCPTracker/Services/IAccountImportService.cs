using DCPTracker.Models;

namespace DCPTracker.Services;

public interface IAccountImportService
{
	IReadOnlyList<RetirementAccount> ParseAccounts(string pastedText);
}