using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DCPTracker.Models;
using DCPTracker.Services;
using System.Collections.ObjectModel;

namespace DCPTracker.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
	private readonly IDeferralBucketRepository bucketRepository;
	private readonly IDistributionProjectionService projectionService;
	private readonly IGoalsService goalsService;

	public DashboardViewModel(
		IDeferralBucketRepository bucketRepository,
		IDistributionProjectionService projectionService,
		IGoalsService goalsService)
	{
		this.bucketRepository = bucketRepository;
		this.projectionService = projectionService;
		this.goalsService = goalsService;
	}

	[ObservableProperty] private decimal totalCurrentBalance;
	[ObservableProperty] private int bucketCount;
	[ObservableProperty] private decimal currentMonthlyPayout;
	[ObservableProperty] private decimal monthlyDistributionGoal;
	[ObservableProperty] private double goalProgress;
	[ObservableProperty] private string statusMessage = string.Empty;

	public ObservableCollection<string> UpcomingDistributions { get; } = [];

	public string GoalProgressLabel => MonthlyDistributionGoal > 0m
		? $"{CurrentMonthlyPayout:C0} of {MonthlyDistributionGoal:C0} monthly goal"
		: $"{CurrentMonthlyPayout:C0} / month (no goal set)";

	partial void OnCurrentMonthlyPayoutChanged(decimal value) => RefreshGoalProgress();
	partial void OnMonthlyDistributionGoalChanged(decimal value) => RefreshGoalProgress();

	private void RefreshGoalProgress()
	{
		GoalProgress = MonthlyDistributionGoal > 0m
			? Math.Min((double)(CurrentMonthlyPayout / MonthlyDistributionGoal), 1.0)
			: 0.0;
		OnPropertyChanged(nameof(GoalProgressLabel));
	}

	[RelayCommand]
	public async Task LoadAsync()
	{
		var goals = goalsService.Load();
		MonthlyDistributionGoal = goals.MonthlyDistributionGoal;

		var buckets = await bucketRepository.GetBucketsAsync();
		BucketCount = buckets.Count;
		TotalCurrentBalance = buckets.Sum(b =>
			projectionService.GetProjectedBalance(b, DateTime.Today, goals.DefaultExpectedAnnualReturnRate));

		CurrentMonthlyPayout = projectionService.GetTotalMonthlyDistributionAt(
			buckets, DateTime.Today, goals.DefaultExpectedAnnualReturnRate);

		UpcomingDistributions.Clear();
		var upcoming = buckets
			.Where(b => b.DistributionStartDate >= DateTime.Today)
			.OrderBy(b => b.DistributionStartDate)
			.Take(5);

		foreach (var b in upcoming)
		{
			var monthly = projectionService.GetEstimatedMonthlyDistribution(b, goals.DefaultExpectedAnnualReturnRate);
			UpcomingDistributions.Add($"{b.Label} — starts {b.DistributionStartLabel}, ~{monthly:C0}/mo for {b.DistributionMonths} months");
		}

		StatusMessage = buckets.Count == 0
			? "Add deferral buckets on the Buckets tab to get started."
			: $"Last refreshed {DateTime.Now:h:mm tt}";
	}
}
