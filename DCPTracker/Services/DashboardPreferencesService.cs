using DCPTracker.Models;

namespace DCPTracker.Services;

public sealed class DashboardPreferencesService : IDashboardPreferencesService
{
	public DashboardPreferences Load() => new()
	{
		RetirementAge = Preferences.Default.Get(nameof(DashboardPreferences.RetirementAge), 60),
		AnnualContribution = Convert.ToDecimal(Preferences.Default.Get(nameof(DashboardPreferences.AnnualContribution), 30_000d)),
		ExpectedAnnualGrowthRate = Convert.ToDecimal(Preferences.Default.Get(nameof(DashboardPreferences.ExpectedAnnualGrowthRate), 0.05d)),
		ConcentrationWarningThreshold = Convert.ToDecimal(Preferences.Default.Get(nameof(DashboardPreferences.ConcentrationWarningThreshold), 0.25d)),
		ConcentrationCriticalThreshold = Convert.ToDecimal(Preferences.Default.Get(nameof(DashboardPreferences.ConcentrationCriticalThreshold), 0.40d)),
		TaxWithholdingRate = Convert.ToDecimal(Preferences.Default.Get(nameof(DashboardPreferences.TaxWithholdingRate), 0.24d)),
		TaxBracketLabel = Preferences.Default.Get(nameof(DashboardPreferences.TaxBracketLabel), "24%"),
		ShowAlerts = Preferences.Default.Get(nameof(DashboardPreferences.ShowAlerts), true),
		ShowTimeline = Preferences.Default.Get(nameof(DashboardPreferences.ShowTimeline), true),
		ShowScenarios = Preferences.Default.Get(nameof(DashboardPreferences.ShowScenarios), true)
	};

	public void Save(DashboardPreferences preferences)
	{
		Preferences.Default.Set(nameof(DashboardPreferences.RetirementAge), preferences.RetirementAge);
		Preferences.Default.Set(nameof(DashboardPreferences.AnnualContribution), Convert.ToDouble(preferences.AnnualContribution));
		Preferences.Default.Set(nameof(DashboardPreferences.ExpectedAnnualGrowthRate), Convert.ToDouble(preferences.ExpectedAnnualGrowthRate));
		Preferences.Default.Set(nameof(DashboardPreferences.ConcentrationWarningThreshold), Convert.ToDouble(preferences.ConcentrationWarningThreshold));
		Preferences.Default.Set(nameof(DashboardPreferences.ConcentrationCriticalThreshold), Convert.ToDouble(preferences.ConcentrationCriticalThreshold));
		Preferences.Default.Set(nameof(DashboardPreferences.TaxWithholdingRate), Convert.ToDouble(preferences.TaxWithholdingRate));
		Preferences.Default.Set(nameof(DashboardPreferences.TaxBracketLabel), preferences.TaxBracketLabel);
		Preferences.Default.Set(nameof(DashboardPreferences.ShowAlerts), preferences.ShowAlerts);
		Preferences.Default.Set(nameof(DashboardPreferences.ShowTimeline), preferences.ShowTimeline);
		Preferences.Default.Set(nameof(DashboardPreferences.ShowScenarios), preferences.ShowScenarios);
	}
}