using DCPTracker.Models;
using SQLite;

namespace DCPTracker.Data;

[Table("PlanningEvents")]
public sealed class PlanningEventRecord
{
	[PrimaryKey]
	public string Id { get; set; } = string.Empty;

	public string Title { get; set; } = string.Empty;

	public string EventType { get; set; } = string.Empty;

	public DateTime EventDate { get; set; }

	public decimal EstimatedAmount { get; set; }

	public static PlanningEventRecord FromDomain(PlanningEvent planningEvent) => new()
	{
		Id = planningEvent.Id.ToString("D"),
		Title = planningEvent.Title,
		EventType = planningEvent.EventType,
		EventDate = planningEvent.EventDate,
		EstimatedAmount = planningEvent.EstimatedAmount
	};

	public PlanningEvent ToDomain() => new()
	{
		Id = Guid.Parse(Id),
		Title = Title,
		EventType = EventType,
		EventDate = EventDate,
		EstimatedAmount = EstimatedAmount
	};
}