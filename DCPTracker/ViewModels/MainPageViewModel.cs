using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DCPTracker.Models;
using DCPTracker.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DCPTracker.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
	private readonly IAccountRepository accountRepository;
	private readonly IPlanningEventRepository planningEventRepository;
	private readonly IDashboardPreferencesService dashboardPreferencesService;
	private readonly IAccountImportService accountImportService;
	private readonly IAlertService alertService;
	private readonly IScenarioProjectionService scenarioProjectionService;
	private readonly IThemePreferenceService themePreferenceService;
	private readonly IDemoModeService demoModeService;
	private DashboardPreferences dashboardPreferences;
	private bool isReloadRequested;

	public MainPageViewModel(IAccountRepository accountRepository, IPlanningEventRepository planningEventRepository, IDashboardPreferencesService dashboardPreferencesService, IAccountImportService accountImportService, IAlertService alertService, IScenarioProjectionService scenarioProjectionService, IThemePreferenceService themePreferenceService, IDemoModeService demoModeService)
	{
		this.accountRepository = accountRepository;
		this.planningEventRepository = planningEventRepository;
		this.dashboardPreferencesService = dashboardPreferencesService;
		this.accountImportService = accountImportService;
		this.alertService = alertService;
		this.scenarioProjectionService = scenarioProjectionService;
		this.themePreferenceService = themePreferenceService;
		this.demoModeService = demoModeService;
		dashboardPreferences = dashboardPreferencesService.Load();
		SelectedAccountType = SupportedAccountTypes[0];
		SelectedEventType = SupportedEventTypes[0];
		EventDate = DateTime.Today.AddMonths(3);
		ApplyPreferencesToEditor();
		RequestDashboardReload();
	}

	public ObservableCollection<RetirementAccount> Accounts { get; } = [];
	public ObservableCollection<PlanningEvent> PlanningEvents { get; } = [];
	public ObservableCollection<DashboardAlert> Alerts { get; } = [];
	public ObservableCollection<ScenarioProjection> Scenarios { get; } = [];
	public IReadOnlyList<string> SupportedAccountTypes { get; } = ["401(k)", "DCP", "RSU", "ESPP", "Brokerage"];
	public IReadOnlyList<string> SupportedEventTypes { get; } = ["Vest", "Payout", "Election", "Limit reminder"];
	public IReadOnlyList<string> TaxBracketOptions { get; } = ["12%", "22%", "24%", "32%", "35%", "37%"];
	public IReadOnlyList<string> ThemeOptions => themePreferenceService.ThemeOptions;

	[ObservableProperty] private bool isBusy;
	[ObservableProperty] private string validationMessage = string.Empty;
	[ObservableProperty] private string statusMessage = "Educational planning model. Confirm elections and official limits with plan materials.";
	[ObservableProperty] private decimal totalTrackedBalance;
	[ObservableProperty] private int trackedAccountCount;
	[ObservableProperty] private decimal employerStockConcentration;
	[ObservableProperty] private string newAccountName = string.Empty;
	[ObservableProperty] private string newAccountBalance = string.Empty;
	[ObservableProperty] private string selectedAccountType = string.Empty;
	[ObservableProperty] private RetirementAccount? selectedAccount;
	[ObservableProperty] private string eventTitle = string.Empty;
	[ObservableProperty] private string selectedEventType = string.Empty;
	[ObservableProperty] private DateTime eventDate;
	[ObservableProperty] private string eventEstimatedAmount = string.Empty;
	[ObservableProperty] private string importText = string.Empty;
	[ObservableProperty] private string importSummary = "Paste rows like Name, DCP, 42000.";
	[ObservableProperty] private string retirementAgeInput = string.Empty;
	[ObservableProperty] private string annualContributionInput = string.Empty;
	[ObservableProperty] private string expectedGrowthPercentInput = string.Empty;
	[ObservableProperty] private string concentrationWarningPercentInput = string.Empty;
	[ObservableProperty] private string concentrationCriticalPercentInput = string.Empty;
	[ObservableProperty] private string selectedTaxBracket = string.Empty;
	[ObservableProperty] private bool showAlerts;
	[ObservableProperty] private bool showTimeline;
	[ObservableProperty] private bool showScenarios;
	[ObservableProperty] private string selectedTheme = "System";
	[ObservableProperty] private bool isDemoModeEnabled;

	public string DashboardSummary => TrackedAccountCount == 0 ? "Add or import accounts to start your retirement snapshot." : $"{TrackedAccountCount} accounts tracked across your planning view.";
	public string ConcentrationSummary => $"{EmployerStockConcentration:P0} employer equity concentration";
	public string AccountEditorTitle => SelectedAccount is null ? "Add account snapshot" : "Edit selected account";
	public string AccountSaveLabel => SelectedAccount is null ? "Add account" : "Save account";

	partial void OnTrackedAccountCountChanged(int value) => OnPropertyChanged(nameof(DashboardSummary));
	partial void OnEmployerStockConcentrationChanged(decimal value) => OnPropertyChanged(nameof(ConcentrationSummary));
	partial void OnSelectedAccountChanged(RetirementAccount? value)
	{
		OnPropertyChanged(nameof(AccountEditorTitle));
		OnPropertyChanged(nameof(AccountSaveLabel));
		if (value is null) return;
		NewAccountName = value.Name;
		NewAccountBalance = value.Balance.ToString("0.##", CultureInfo.CurrentCulture);
		SelectedAccountType = value.AccountTypeLabel;
		ValidationMessage = string.Empty;
	}

	partial void OnSelectedThemeChanged(string value) => themePreferenceService.SaveAndApply(value);
	partial void OnIsDemoModeEnabledChanged(bool value)
	{
		demoModeService.SetDemoMode(value);
		StatusMessage = value ? "Demo Mode is on. Edits stay in memory for walkthroughs." : "Live local mode is on. SQLite-backed records are shown.";
		RequestDashboardReload();
	}

	[RelayCommand]
	private async Task LoadDashboardAsync()
	{
		if (IsBusy)
		{
			isReloadRequested = true;
			return;
		}

		try
		{
			IsBusy = true;
			do
			{
				isReloadRequested = false;
				Replace(Accounts, await accountRepository.GetAccountsAsync());
				Replace(PlanningEvents, await planningEventRepository.GetEventsAsync());
				RefreshDashboardOutputs();
			}
			while (isReloadRequested);
		}
		catch
		{
			StatusMessage = "Unable to refresh data right now. Try again in a moment.";
		}
		finally { IsBusy = false; }
	}

	[RelayCommand]
	private async Task SaveAccountAsync()
	{
		if (string.IsNullOrWhiteSpace(NewAccountName)) { ValidationMessage = "Enter an account name."; return; }
		if (!TryParseNonNegativeDecimal(NewAccountBalance, out var balance)) { ValidationMessage = "Enter a valid non-negative account balance."; return; }
		await accountRepository.SaveAccountAsync(new RetirementAccount { Id = SelectedAccount?.Id ?? Guid.NewGuid(), Name = NewAccountName.Trim(), AccountType = ParseAccountType(SelectedAccountType), Balance = balance, UpdatedAt = DateTimeOffset.UtcNow });
		StatusMessage = SelectedAccount is null ? "Account added." : "Account updated.";
		ClearAccountEditor();
		await LoadDashboardAsync();
	}

	[RelayCommand]
	private async Task DeleteSelectedAccountAsync()
	{
		if (SelectedAccount is null) { ValidationMessage = "Select an account before deleting."; return; }
		await accountRepository.DeleteAccountAsync(SelectedAccount.Id);
		StatusMessage = "Account deleted.";
		ClearAccountEditor();
		await LoadDashboardAsync();
	}

	[RelayCommand] private void StartNewAccount() => ClearAccountEditor();

	[RelayCommand]
	private async Task ImportAccountsAsync()
	{
		var importedAccounts = accountImportService.ParseAccounts(ImportText);
		if (importedAccounts.Count == 0) { ImportSummary = "No rows imported. Use Name, Type, Balance separated by commas, tabs, or pipes."; return; }
		foreach (var account in importedAccounts) await accountRepository.SaveAccountAsync(account);
		ImportSummary = $"Imported {importedAccounts.Count} account snapshots.";
		ImportText = string.Empty;
		await LoadDashboardAsync();
	}

	[RelayCommand]
	private async Task SavePlanningEventAsync()
	{
		if (string.IsNullOrWhiteSpace(EventTitle)) { ValidationMessage = "Enter a title for the timeline event."; return; }
		if (!TryParseNonNegativeDecimal(EventEstimatedAmount, out var estimatedAmount)) { ValidationMessage = "Enter a valid non-negative estimated event amount."; return; }
		await planningEventRepository.SaveEventAsync(new PlanningEvent { Id = Guid.NewGuid(), Title = EventTitle.Trim(), EventType = SelectedEventType, EventDate = EventDate.Date, EstimatedAmount = estimatedAmount });
		EventTitle = string.Empty;
		EventEstimatedAmount = string.Empty;
		ValidationMessage = string.Empty;
		StatusMessage = "Timeline event added.";
		await LoadDashboardAsync();
	}

	[RelayCommand]
	private void SavePlanningAssumptions()
	{
		if (!int.TryParse(RetirementAgeInput, NumberStyles.Integer, CultureInfo.CurrentCulture, out var retirementAge) || retirementAge < 41 || retirementAge > 90) { ValidationMessage = "Retirement age must be between 41 and 90."; return; }
		if (!TryParseNonNegativeDecimal(AnnualContributionInput, out var annualContribution) || !TryParsePercent(ExpectedGrowthPercentInput, out var growthRate) || !TryParsePercent(ConcentrationWarningPercentInput, out var warningThreshold) || !TryParsePercent(ConcentrationCriticalPercentInput, out var criticalThreshold) || warningThreshold >= criticalThreshold) { ValidationMessage = "Check contribution, percent inputs, and keep watch threshold below critical."; return; }
		dashboardPreferences = new DashboardPreferences { RetirementAge = retirementAge, AnnualContribution = annualContribution, ExpectedAnnualGrowthRate = growthRate, ConcentrationWarningThreshold = warningThreshold, ConcentrationCriticalThreshold = criticalThreshold, TaxBracketLabel = SelectedTaxBracket, TaxWithholdingRate = ParseTaxBracket(SelectedTaxBracket), ShowAlerts = ShowAlerts, ShowTimeline = ShowTimeline, ShowScenarios = ShowScenarios };
		dashboardPreferencesService.Save(dashboardPreferences);
		ValidationMessage = string.Empty;
		StatusMessage = "Planning assumptions saved locally.";
		RefreshDashboardOutputs();
	}

	private void ApplyPreferencesToEditor()
	{
		RetirementAgeInput = dashboardPreferences.RetirementAge.ToString(CultureInfo.CurrentCulture);
		AnnualContributionInput = dashboardPreferences.AnnualContribution.ToString("0.##", CultureInfo.CurrentCulture);
		ExpectedGrowthPercentInput = (dashboardPreferences.ExpectedAnnualGrowthRate * 100m).ToString("0.##", CultureInfo.CurrentCulture);
		ConcentrationWarningPercentInput = (dashboardPreferences.ConcentrationWarningThreshold * 100m).ToString("0.##", CultureInfo.CurrentCulture);
		ConcentrationCriticalPercentInput = (dashboardPreferences.ConcentrationCriticalThreshold * 100m).ToString("0.##", CultureInfo.CurrentCulture);
		SelectedTaxBracket = dashboardPreferences.TaxBracketLabel;
		SelectedTheme = themePreferenceService.CurrentTheme;
		IsDemoModeEnabled = demoModeService.IsDemoModeEnabled;
		ShowAlerts = dashboardPreferences.ShowAlerts;
		ShowTimeline = dashboardPreferences.ShowTimeline;
		ShowScenarios = dashboardPreferences.ShowScenarios;
	}

	private void RefreshDashboardOutputs()
	{
		TrackedAccountCount = Accounts.Count;
		TotalTrackedBalance = Accounts.Sum(account => account.Balance);
		var equityBalance = Accounts.Where(account => account.AccountType is AccountType.RestrictedStockUnits or AccountType.EmployeeStockPurchasePlan).Sum(account => account.Balance);
		EmployerStockConcentration = TotalTrackedBalance == 0m ? 0m : equityBalance / TotalTrackedBalance;
		Replace(Alerts, alertService.BuildAlerts([.. Accounts], [.. PlanningEvents], dashboardPreferences));
		Replace(Scenarios, scenarioProjectionService.BuildScenarios([.. Accounts], dashboardPreferences));
	}

	private void ClearAccountEditor() { SelectedAccount = null; NewAccountName = string.Empty; NewAccountBalance = string.Empty; SelectedAccountType = SupportedAccountTypes[0]; ValidationMessage = string.Empty; }
	private void RequestDashboardReload() => _ = LoadDashboardAsync();
	private static void Replace<T>(ObservableCollection<T> collection, IEnumerable<T> items) { collection.Clear(); foreach (var item in items) collection.Add(item); }
	private static bool TryParseNonNegativeDecimal(string value, out decimal result) => decimal.TryParse(value, NumberStyles.Currency, CultureInfo.CurrentCulture, out result) && result >= 0m;
	private static bool TryParsePercent(string value, out decimal result) { if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var percent) && percent >= 0m) { result = percent / 100m; return true; } result = 0m; return false; }
	private static decimal ParseTaxBracket(string taxBracketLabel) => decimal.TryParse(taxBracketLabel.TrimEnd('%'), NumberStyles.Number, CultureInfo.InvariantCulture, out var taxRate) ? taxRate / 100m : 0.24m;
	private static AccountType ParseAccountType(string accountTypeLabel) => accountTypeLabel switch { "DCP" => AccountType.DeferredCompensation, "RSU" => AccountType.RestrictedStockUnits, "ESPP" => AccountType.EmployeeStockPurchasePlan, "Brokerage" => AccountType.Brokerage, _ => AccountType.Retirement401K };
}