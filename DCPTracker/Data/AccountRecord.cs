using DCPTracker.Models;
using SQLite;

namespace DCPTracker.Data;

[Table("Accounts")]
public sealed class AccountRecord
{
	[PrimaryKey]
	public string Id { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public int AccountTypeValue { get; set; }

	public decimal Balance { get; set; }

	public DateTime UpdatedAtUtc { get; set; }

	public static AccountRecord FromDomain(RetirementAccount account) => new()
	{
		Id = account.Id.ToString("D"),
		Name = account.Name,
		AccountTypeValue = (int)account.AccountType,
		Balance = account.Balance,
		UpdatedAtUtc = account.UpdatedAt.UtcDateTime
	};

	public RetirementAccount ToDomain() => new()
	{
		Id = Guid.Parse(Id),
		Name = Name,
		AccountType = (AccountType)AccountTypeValue,
		Balance = Balance,
		UpdatedAt = new DateTimeOffset(DateTime.SpecifyKind(UpdatedAtUtc, DateTimeKind.Utc))
	};
}