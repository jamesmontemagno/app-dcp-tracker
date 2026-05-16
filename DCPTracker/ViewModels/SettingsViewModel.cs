using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DCPTracker.Models;
using DCPTracker.Services;
using System.Globalization;

namespace DCPTracker.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
	private readonly IGoalsService goalsService;
	private readonly IThemePreferenceService themePreferenceService;

	public SettingsViewModel(IGoalsService goalsService, IThemePreferenceService themePreferenceService)
	{
		this.goalsService = goalsService;
		this.themePreferenceService = themePreferenceService;
		Load();
	}

	[ObservableProperty] private string monthlyGoalInput = string.Empty;
	[ObservableProperty] private string defaultReturnRateInput = string.Empty;
	[ObservableProperty] private string taxWithholdingRateInput = string.Empty;
	[ObservableProperty] private string selectedTheme = "System";
	[ObservableProperty] private string validationMessage = string.Empty;
	[ObservableProperty] private string statusMessage = string.Empty;

	public IReadOnlyList<string> ThemeOptions => themePreferenceService.ThemeOptions;

	partial void OnSelectedThemeChanged(string value) => themePreferenceService.SaveAndApply(value);

	private void Load()
	{
		var goals = goalsService.Load();
		MonthlyGoalInput = goals.MonthlyDistributionGoal.ToString("0.##", CultureInfo.CurrentCulture);
		DefaultReturnRateInput = (goals.DefaultExpectedAnnualReturnRate * 100m).ToString("0.##", CultureInfo.CurrentCulture);
		TaxWithholdingRateInput = (goals.TaxWithholdingRate * 100m).ToString("0.##", CultureInfo.CurrentCulture);
		SelectedTheme = themePreferenceService.CurrentTheme;
	}

	[RelayCommand]
	private void SaveGoals()
	{
		if (!decimal.TryParse(MonthlyGoalInput, NumberStyles.Currency, CultureInfo.CurrentCulture, out var goal) || goal < 0m)
		{
			ValidationMessage = "Enter a valid monthly distribution goal.";
			return;
		}

		if (!decimal.TryParse(DefaultReturnRateInput, NumberStyles.Number, CultureInfo.CurrentCulture, out var returnRate) || returnRate < 0m)
		{
			ValidationMessage = "Enter a valid default expected annual return %.";
			return;
		}

		if (!decimal.TryParse(TaxWithholdingRateInput, NumberStyles.Number, CultureInfo.CurrentCulture, out var taxRate) || taxRate < 0m || taxRate >= 100m)
		{
			ValidationMessage = "Tax withholding must be between 0 and 100.";
			return;
		}

		goalsService.Save(new AppGoals
		{
			MonthlyDistributionGoal = goal,
			DefaultExpectedAnnualReturnRate = returnRate / 100m,
			TaxWithholdingRate = taxRate / 100m
		});

		ValidationMessage = string.Empty;
		StatusMessage = "Settings saved.";
	}
}
