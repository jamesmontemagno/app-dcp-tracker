using SQLite;

namespace DCPTracker.Data;

public static class LocalDatabaseOptions
{
	public const string DatabaseFilename = "dcp-tracker.db3";

	public const SQLiteOpenFlags Flags =
		SQLiteOpenFlags.ReadWrite |
		SQLiteOpenFlags.Create |
		SQLiteOpenFlags.SharedCache;

	public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
}