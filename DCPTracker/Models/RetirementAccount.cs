namespace DCPTracker.Models;

public sealed class RetirementAccount
{
	public required Guid Id { get; init; }

	public required string Name { get; init; }

	public required AccountType AccountType { get; init; }

	public required decimal Balance { get; init; }

	public required DateTimeOffset UpdatedAt { get; init; }

	public string AccountTypeLabel => AccountType switch
	{
		AccountType.Retirement401K => "401(k)",
		AccountType.DeferredCompensation => "DCP",
		AccountType.RestrictedStockUnits => "RSU",
		AccountType.EmployeeStockPurchasePlan => "ESPP",
		_ => "Brokerage"
	};
}