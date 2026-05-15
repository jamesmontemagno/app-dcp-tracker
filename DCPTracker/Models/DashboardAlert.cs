namespace DCPTracker.Models;

public sealed class DashboardAlert
{
	public required string Severity { get; init; }

	public required string Title { get; init; }

	public required string Detail { get; init; }
}