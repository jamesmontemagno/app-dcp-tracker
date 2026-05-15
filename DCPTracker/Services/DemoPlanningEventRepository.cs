using DCPTracker.Models;

namespace DCPTracker.Services;

public sealed class DemoPlanningEventRepository : IPlanningEventRepository
{
	private readonly List<PlanningEvent> events =
	[
		Create("Quarterly RSU vest", "Vest", DateTime.Today.AddDays(28), 19_750m),
		Create("DCP annual election review", "Election", DateTime.Today.AddMonths(4), 0m),
		Create("Year-end contribution limit check", "Limit reminder", DateTime.Today.AddMonths(6), 0m),
		Create("Deferred payout tranche", "Payout", DateTime.Today.AddYears(1), 24_000m)
	];

	public Task<IReadOnlyList<PlanningEvent>> GetEventsAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		return Task.FromResult<IReadOnlyList<PlanningEvent>>([.. events.OrderBy(planningEvent => planningEvent.EventDate)]);
	}

	public Task SaveEventAsync(PlanningEvent planningEvent, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		events.Add(planningEvent);
		return Task.CompletedTask;
	}

	public Task DeleteEventAsync(Guid planningEventId, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		events.RemoveAll(planningEvent => planningEvent.Id == planningEventId);
		return Task.CompletedTask;
	}

	private static PlanningEvent Create(string title, string eventType, DateTime eventDate, decimal estimatedAmount) => new() { Id = Guid.NewGuid(), Title = title, EventType = eventType, EventDate = eventDate, EstimatedAmount = estimatedAmount };
}