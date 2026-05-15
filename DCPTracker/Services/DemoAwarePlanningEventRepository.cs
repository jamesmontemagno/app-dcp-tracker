using DCPTracker.Models;

namespace DCPTracker.Services;

public sealed class DemoAwarePlanningEventRepository : IPlanningEventRepository
{
	private readonly SqlitePlanningEventRepository sqliteRepository;
	private readonly DemoPlanningEventRepository demoRepository;
	private readonly IDemoModeService demoModeService;

	public DemoAwarePlanningEventRepository(SqlitePlanningEventRepository sqliteRepository, DemoPlanningEventRepository demoRepository, IDemoModeService demoModeService)
	{
		this.sqliteRepository = sqliteRepository;
		this.demoRepository = demoRepository;
		this.demoModeService = demoModeService;
	}

	public Task<IReadOnlyList<PlanningEvent>> GetEventsAsync(CancellationToken cancellationToken = default) => ActiveRepository.GetEventsAsync(cancellationToken);
	public Task SaveEventAsync(PlanningEvent planningEvent, CancellationToken cancellationToken = default) => ActiveRepository.SaveEventAsync(planningEvent, cancellationToken);
	public Task DeleteEventAsync(Guid planningEventId, CancellationToken cancellationToken = default) => ActiveRepository.DeleteEventAsync(planningEventId, cancellationToken);
	private IPlanningEventRepository ActiveRepository => demoModeService.IsDemoModeEnabled ? demoRepository : sqliteRepository;
}