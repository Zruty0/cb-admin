namespace CbAdmin;


/// <summary>
/// Information about one ban.
/// </summary>
public class BanInfo
{
    public required int Id { get; set; }
    public required string PlayerId { get; set; }
    public required string PlayerName { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime ExpiryTime { get; set; }
    public required string Reason { get; set; }
    public required string AdminName { get; set; }

    // Presentation functions.
    public string StartTimeStr => StartTime.ToString("G");
    public string ExpiryTimeStr => ExpiryTime.ToString("G");
    public string RemainingTimeStr => ExpiryTime < DateTime.Now ? "expired" : (ExpiryTime - DateTime.Now).ToString("g");
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