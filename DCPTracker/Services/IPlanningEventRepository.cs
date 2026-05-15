using DCPTracker.Models;

namespace DCPTracker.Services;

public interface IPlanningEventRepository
{
	Task<IReadOnlyList<PlanningEvent>> GetEventsAsync(CancellationToken cancellationToken = default);

	Task SaveEventAsync(PlanningEvent planningEvent, CancellationToken cancellationToken = default);

	Task DeleteEventAsync(Guid planningEventId, CancellationToken cancellationToken = default);
}