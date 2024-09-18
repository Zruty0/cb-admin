namespace CbAdmin;
using NpgsqlTypes;

/// <summary>
/// Information about one ban.
/// </summary>
public class BanInfo
{
    public required int Id { get; set; }
    public required int PlayerId { get; set; }
    public required string PlayerName { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime ExpiryTime { get; set; }
    public required string Reason { get; set; }
    public required string AdminName { get; set; }

    // Presentation functions.
    public string StartTimeStr => StartTime.ToString("G");
    public string ExpiryTimeStr => ExpiryTime.ToString("G");
    public string RemainingTimeStr => ExpiryTime < DateTime.Now ? "expired" : (ExpiryTime - DateTime.Now).ToString("g");
    public int RemainingTimeMinutes => ExpiryTime < DateTime.Now ? 0 : (int)(ExpiryTime - DateTime.Now).TotalMinutes;
}

/// <summary>
/// Information about one player's alias.
/// </summary>
public class PlayerAliasInfo
{
    public required string Alias { get; set; }
    public required DateTime LastUsed { get; set; }

    // Presentation functions.
    public string LastUsedStr => LastUsed.ToString("G");
}

/// <summary>
/// Predefined reason/duration combinations.
/// </summary>
public class BanPresetInfo
{
    public string Reason { get; set; } = null!;
    public float DurationSecondsRaw {get;set;}

    // Presentation functions.
    public TimeSpan Duration => TimeSpan.FromSeconds(DurationSecondsRaw);
    public string DurationStr => Duration.TotalDays > 3650 ? "permanent" : Duration.ToString("g");
    public int DurationMinutes => (int)Duration.TotalMinutes;
}

/// <summary>
/// All information about one player from the ban server.
/// </summary>
public class PlayerInfo
{
    public required int Id { get; set; }
    /// <summary>
    /// All the logins this player has used, and when.
    /// </summary>
    public required PlayerAliasInfo[] Aliases { get; set; }

    /// <summary>
    /// All the user's bans in the system, ordered from latest to oldest.
    /// </summary>
    public required BanInfo[] Bans { get; set; }

}