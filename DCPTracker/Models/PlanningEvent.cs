namespace DCPTracker.Models;

public sealed class PlanningEvent
{
	public required Guid Id { get; init; }

	public required string Title { get; init; }

	public required string EventType { get; init; }

	public required DateTime EventDate { get; init; }

	public required decimal EstimatedAmount { get; init; }

	public string EventDateLabel => EventDate.ToString("MMM d, yyyy");
}