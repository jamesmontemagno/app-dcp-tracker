using DCPTracker.Data;
using DCPTracker.Models;
using SQLite;

namespace DCPTracker.Services;

public sealed class SqlitePlanningEventRepository : IPlanningEventRepository
{
	private readonly SemaphoreSlim initializationGate = new(1, 1);
	private SQLiteAsyncConnection? database;

	public async Task<IReadOnlyList<PlanningEvent>> GetEventsAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var connection = await GetDatabaseAsync(cancellationToken);
		var records = await connection.Table<PlanningEventRecord>()
			.OrderBy(record => record.EventDate)
			.ToListAsync();

		return [.. records.Select(record => record.ToDomain())];
	}

	public async Task SaveEventAsync(PlanningEvent planningEvent, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var connection = await GetDatabaseAsync(cancellationToken);
		await connection.InsertOrReplaceAsync(PlanningEventRecord.FromDomain(planningEvent));
	}

	public async Task DeleteEventAsync(Guid planningEventId, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var connection = await GetDatabaseAsync(cancellationToken);
		await connection.DeleteAsync<PlanningEventRecord>(planningEventId.ToString("D"));
	}

	private async Task<SQLiteAsyncConnection> GetDatabaseAsync(CancellationToken cancellationToken)
	{
		if (database is not null)
		{
			return database;
		}

		await initializationGate.WaitAsync(cancellationToken);
		try
		{
			if (database is not null)
			{
				return database;
			}

			var connection = new SQLiteAsyncConnection(LocalDatabaseOptions.DatabasePath, LocalDatabaseOptions.Flags);
			await connection.CreateTableAsync<PlanningEventRecord>();
			await SeedStarterEventsAsync(connection);
			database = connection;
			return database;
		}
		finally
		{
			initializationGate.Release();
		}
	}

	private static async Task SeedStarterEventsAsync(SQLiteAsyncConnection connection)
	{
		if (await connection.Table<PlanningEventRecord>().CountAsync() > 0)
		{
			return;
		}

		var today = DateTime.Today;
		var starterEvents = new[]
		{
			CreateEvent("Spring RSU vest", "Vest", today.AddMonths(2), 12_500m),
			CreateEvent("DCP election review", "Election", today.AddMonths(5), 0m),
			CreateEvent("Estimated payout checkpoint", "Payout", today.AddYears(1), 18_000m)
		};

		await connection.InsertAllAsync(starterEvents.Select(PlanningEventRecord.FromDomain));
	}

	private static PlanningEvent CreateEvent(string title, string eventType, DateTime eventDate, decimal estimatedAmount) => new()
	{
		Id = Guid.NewGuid(),
		Title = title,
		EventType = eventType,
		EventDate = eventDate,
		EstimatedAmount = estimatedAmount
	};
}