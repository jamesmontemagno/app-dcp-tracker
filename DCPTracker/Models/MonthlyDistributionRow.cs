namespace DCPTracker.Models;

public sealed class MonthlyDistributionRow
{
	public required DateTime Month { get; init; }

	public required decimal GrossAmount { get; init; }

	public required decimal NetAmount { get; init; }

	public required decimal CumulativeNet { get; init; }

	public string MonthLabel => Month.ToString("MMM yyyy");

	public string GrossLabel => GrossAmount.ToString("C0");

	public string NetLabel => NetAmount.ToString("C0");

	public string CumulativeNetLabel => CumulativeNet.ToString("C0");
}
