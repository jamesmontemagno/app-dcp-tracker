using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DCPTracker.Models;
using DCPTracker.Services;
using System.Collections.ObjectModel;

namespace DCPTracker.ViewModels;

public partial class ProjectionsViewModel : ObservableObject
{
	private readonly IDeferralBucketRepository bucketRepository;
	private readonly IDistributionProjectionService projectionService;
	private readonly IGoalsService goalsService;

	public ProjectionsViewModel(
		IDeferralBucketRepository bucketRepository,
		IDistributionProjectionService projectionService,
		IGoalsService goalsService)
	{
		this.bucketRepository = bucketRepository;
		this.projectionService = projectionService;
		this.goalsService = goalsService;
	}

	[ObservableProperty] private bool showTaxAdjusted = true;
	[ObservableProperty] private decimal weightedAverageReturn;
	[ObservableProperty] private string weightedAverageReturnLabel = string.Empty;
	[ObservableProperty] private string summaryMessage = string.Empty;

	public ObservableCollection<MonthlyDistributionRow> TimelineRows { get; } = [];

	partial void OnShowTaxAdjustedChanged(bool value)
	{
		OnPropertyChanged(nameof(AmountColumnHeader));
	}

	public string AmountColumnHeader => ShowTaxAdjusted ? "Net Amount" : "Gross Amount";

	[RelayCommand]
	public async Task LoadAsync()
	{
		var goals = goalsService.Load();
		var buckets = await bucketRepository.GetBucketsAsync();

		WeightedAverageReturn = projectionService.GetWeightedAverageReturnRate(buckets, goals.DefaultExpectedAnnualReturnRate);
		WeightedAverageReturnLabel = $"Weighted avg. expected return: {WeightedAverageReturn:P1}";

		var timeline = projectionService.BuildDistributionTimeline(buckets, goals);
		TimelineRows.Clear();
		foreach (var row in timeline)
		{
			TimelineRows.Add(row);
		}

		SummaryMessage = timeline.Count == 0
			? "No distribution months found. Add buckets with future distribution dates."
			: $"{timeline.Count} distribution months across {buckets.Count} buckets.";
	}
}
