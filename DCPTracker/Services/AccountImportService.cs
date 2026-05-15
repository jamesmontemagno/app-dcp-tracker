using DCPTracker.Models;
using System.Globalization;

namespace DCPTracker.Services;

public sealed class AccountImportService : IAccountImportService
{
	public IReadOnlyList<RetirementAccount> ParseAccounts(string pastedText)
	{
		var accounts = new List<RetirementAccount>();
		var rows = pastedText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		foreach (var row in rows)
		{
			if (row.Contains("name", StringComparison.OrdinalIgnoreCase) && row.Contains("balance", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			var columns = row.Split([',', '\t', '|'], StringSplitOptions.TrimEntries);
			if (columns.Length < 3 || !decimal.TryParse(columns[2], NumberStyles.Currency, CultureInfo.CurrentCulture, out var balance))
			{
				continue;
			}

			accounts.Add(new RetirementAccount
			{
				Id = Guid.NewGuid(),
				Name = columns[0],
				AccountType = ParseAccountType(columns[1]),
				Balance = Math.Max(balance, 0m),
				UpdatedAt = DateTimeOffset.UtcNow
			});
		}

		return accounts;
	}

	private static AccountType ParseAccountType(string accountType) => accountType.Trim().ToUpperInvariant() switch
	{
		"DCP" => AccountType.DeferredCompensation,
		"RSU" => AccountType.RestrictedStockUnits,
		"ESPP" => AccountType.EmployeeStockPurchasePlan,
		"BROKERAGE" => AccountType.Brokerage,
		_ => AccountType.Retirement401K
	};
}