using DCPTracker.Models;

namespace DCPTracker.Services;

public interface IDashboardPreferencesService
{
	DashboardPreferences Load();

	void Save(DashboardPreferences preferences);
}