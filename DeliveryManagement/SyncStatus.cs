namespace Detrack.Data
{
	public enum SyncStatus
	{
		New = 0,
		Processing = 1,
		Edit = 2,
		Delete = 3,
		Deleted = 4,
		Completed = 9,
		Error = 10
	}
}