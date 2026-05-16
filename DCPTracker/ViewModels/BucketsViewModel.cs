using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DCPTracker.Models;
using DCPTracker.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DCPTracker.ViewModels;

public partial class BucketsViewModel : ObservableObject
{
	private readonly IDeferralBucketRepository bucketRepository;
	private readonly IDistributionProjectionService projectionService;
	private readonly IGoalsService goalsService;

	public BucketsViewModel(
		IDeferralBucketRepository bucketRepository,
		IDistributionProjectionService projectionService,
		IGoalsService goalsService)
	{
		this.bucketRepository = bucketRepository;
		this.projectionService = projectionService;
		this.goalsService = goalsService;
		DistributionStartDate = DateTime.Today.AddYears(5);
	}

	public ObservableCollection<BucketDisplayItem> Buckets { get; } = [];

	// ── Editor fields ─────────────────────────────────────────────────────────
	[ObservableProperty] private DeferralBucket? selectedBucket;
	[ObservableProperty] private BucketDisplayItem? selectedDisplayItem;
	[ObservableProperty] private string labelInput = string.Empty;
	[ObservableProperty] private string deferralYearInput = string.Empty;
	[ObservableProperty] private string deferredAmountInput = string.Empty;
	[ObservableProperty] private DateTime distributionStartDate;
	[ObservableProperty] private string distributionMonthsInput = string.Empty;
	[ObservableProperty] private string validationMessage = string.Empty;
	[ObservableProperty] private string statusMessage = string.Empty;

	// ── Inline investment editor ───────────────────────────────────────────────
	[ObservableProperty] private string fundNameInput = string.Empty;
	[ObservableProperty] private string allocationPercentInput = string.Empty;
	[ObservableProperty] private string annualReturnInput = string.Empty;

	public ObservableCollection<InvestmentAllocation> PendingInvestments { get; } = [];

	public string EditorTitle => SelectedBucket is null ? "Add deferral bucket" : "Edit bucket";
	public string SaveLabel => SelectedBucket is null ? "Add bucket" : "Save changes";

	partial void OnSelectedDisplayItemChanged(BucketDisplayItem? value) => SelectedBucket = value?.Bucket;

	partial void OnSelectedBucketChanged(DeferralBucket? value)
	{
		OnPropertyChanged(nameof(EditorTitle));
		OnPropertyChanged(nameof(SaveLabel));
		if (value is null)
		{
			return;
		}

		LabelInput = value.Label;
		DeferralYearInput = value.DeferralYear.ToString(CultureInfo.CurrentCulture);
		DeferredAmountInput = value.DeferredAmount.ToString("0.##", CultureInfo.CurrentCulture);
		DistributionStartDate = value.DistributionStartDate;
		DistributionMonthsInput = value.DistributionMonths.ToString(CultureInfo.CurrentCulture);
		PendingInvestments.Clear();
		foreach (var inv in value.Investments)
		{
			PendingInvestments.Add(inv);
		}

		ValidationMessage = string.Empty;
	}

	[RelayCommand]
	public async Task LoadAsync()
	{
		var goals = goalsService.Load();
		var buckets = await bucketRepository.GetBucketsAsync();
		Buckets.Clear();
		foreach (var b in buckets)
		{
			var projected = projectionService.GetProjectedBalance(b, DateTime.Today, goals.DefaultExpectedAnnualReturnRate);
			var monthly = projectionService.GetEstimatedMonthlyDistribution(b, goals.DefaultExpectedAnnualReturnRate);
			Buckets.Add(new BucketDisplayItem(b, projected, monthly));
		}
	}

	[RelayCommand]
	private void AddInvestment()
	{
		if (string.IsNullOrWhiteSpace(FundNameInput))
		{
			ValidationMessage = "Enter a fund name.";
			return;
		}

		if (!decimal.TryParse(AllocationPercentInput, NumberStyles.Number, CultureInfo.CurrentCulture, out var alloc) || alloc <= 0m || alloc > 100m)
		{
			ValidationMessage = "Allocation must be between 0 and 100.";
			return;
		}

		if (!decimal.TryParse(AnnualReturnInput, NumberStyles.Number, CultureInfo.CurrentCulture, out var returnRate) || returnRate < 0m)
		{
			ValidationMessage = "Enter a valid expected annual return %.";
			return;
		}

		PendingInvestments.Add(new InvestmentAllocation
		{
			FundName = FundNameInput.Trim(),
			AllocationPercent = alloc,
			ExpectedAnnualReturnRate = returnRate / 100m
		});

		FundNameInput = string.Empty;
		AllocationPercentInput = string.Empty;
		AnnualReturnInput = string.Empty;
		ValidationMessage = string.Empty;
	}

	[RelayCommand]
	private void RemoveInvestment(InvestmentAllocation investment) => PendingInvestments.Remove(investment);

	[RelayCommand]
	private async Task SaveBucketAsync()
	{
		if (string.IsNullOrWhiteSpace(LabelInput))
		{
			ValidationMessage = "Enter a label for this bucket.";
			return;
		}

		if (!int.TryParse(DeferralYearInput, NumberStyles.Integer, CultureInfo.CurrentCulture, out var year) || year < 2000 || year > DateTime.Today.Year + 10)
		{
			ValidationMessage = "Enter a valid deferral year.";
			return;
		}

		if (!decimal.TryParse(DeferredAmountInput, NumberStyles.Currency, CultureInfo.CurrentCulture, out var amount) || amount <= 0m)
		{
			ValidationMessage = "Enter a valid deferred amount.";
			return;
		}

		if (!int.TryParse(DistributionMonthsInput, NumberStyles.Integer, CultureInfo.CurrentCulture, out var months) || months <= 0)
		{
			ValidationMessage = "Enter a valid number of distribution months.";
			return;
		}

		var bucket = new DeferralBucket
		{
			Id = SelectedBucket?.Id ?? Guid.NewGuid(),
			Label = LabelInput.Trim(),
			DeferralYear = year,
			DeferredAmount = amount,
			DistributionStartDate = DistributionStartDate.Date,
			DistributionMonths = months,
			Investments = [.. PendingInvestments]
		};

		await bucketRepository.SaveBucketAsync(bucket);
		StatusMessage = SelectedBucket is null ? "Bucket added." : "Bucket updated.";
		ClearEditor();
		await LoadAsync();
	}

	[RelayCommand]
	private async Task DeleteSelectedBucketAsync()
	{
		if (SelectedBucket is null)
		{
			ValidationMessage = "Select a bucket to delete.";
			return;
		}

		await bucketRepository.DeleteBucketAsync(SelectedBucket.Id);
		StatusMessage = "Bucket deleted.";
		ClearEditor();
		await LoadAsync();
	}

	[RelayCommand]
	private void StartNewBucket() => ClearEditor();

	private void ClearEditor()
	{
		SelectedBucket = null;
		LabelInput = string.Empty;
		DeferralYearInput = string.Empty;
		DeferredAmountInput = string.Empty;
		DistributionStartDate = DateTime.Today.AddYears(5);
		DistributionMonthsInput = string.Empty;
		PendingInvestments.Clear();
		FundNameInput = string.Empty;
		AllocationPercentInput = string.Empty;
		AnnualReturnInput = string.Empty;
		ValidationMessage = string.Empty;
	}
}

public sealed class BucketDisplayItem(DeferralBucket bucket, decimal projectedBalance, decimal estimatedMonthlyDistribution)
{
	public DeferralBucket Bucket { get; } = bucket;
	public decimal ProjectedBalance { get; } = projectedBalance;
	public decimal EstimatedMonthlyDistribution { get; } = estimatedMonthlyDistribution;

	public string Label => Bucket.Label;
	public decimal DeferredAmount => Bucket.DeferredAmount;
	public string DistributionStartLabel => Bucket.DistributionStartLabel;
	public int DistributionMonths => Bucket.DistributionMonths;
	public string ProjectedBalanceLabel => ProjectedBalance.ToString("C0");
	public string EstimatedMonthlyLabel => EstimatedMonthlyDistribution.ToString("C0");
}
