using Npgsql;
using NpgsqlTypes;
using Dapper;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Security.Claims;

namespace CbAdmin;

public class DataClient
{
    public class AdminInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    private string ConnString;

    public DataClient(IConfiguration configuration)
    {
        ConnString = configuration.GetValue<string>("DatabaseConnString")!;
    }

    public AdminInfo? GetAdminInfo(ClaimsPrincipal user)
    {
        var usernameClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        var discriminatorClaim = user.Claims.FirstOrDefault(c => c.Type == "urn:discord:user:discriminator");

        if (usernameClaim == null || discriminatorClaim == null)
        {
            // Discord auth not complete.
            return null;
        }

        var finalName = $"{usernameClaim.Value}#{discriminatorClaim.Value}";
        return DoGetAdminInfo(user.Identity?.Name, finalName);
    }

    private AdminInfo? DoGetAdminInfo(string? shortName, string fullName)
    {
        using var conn = new NpgsqlConnection(ConnString);
        var admin = conn.Query<AdminInfo>("SELECT * FROM admin WHERE LOWER(name)=@name1 OR LOWER(name)=@name2", new { name1 = shortName, name2 = fullName }).AsList().SingleOrDefault();

        // Strip the suffix from Discord name, if present.
        if (admin != null && admin.Name.Contains('#'))
        {
            admin.Name = admin.Name.Substring(0, admin.Name.IndexOf('#'));
        }
        return admin;
    }

    public string? GetLastPlayerName(int playerId)
    {
        using var conn = new NpgsqlConnection(ConnString);
        return DoGetLastPlayerName(playerId, conn);
    }

    private string? DoGetLastPlayerName(int playerId, NpgsqlConnection conn)
    {
        string? name = conn.Query<string?>("select player_nick from player_nick where player_id=@id order by last_used desc", new { id = playerId }).FirstOrDefault();
        return name;
    }

    public BanPresetInfo[] LoadBanPresets()
    {
        using var conn = new NpgsqlConnection(ConnString);
        var presets = conn.Query<BanPresetInfo>("select reason, EXTRACT(EPOCH from default_length) as DurationSecondsRaw from ban_reason").AsList();
        return presets.ToArray();
    }

    public PlayerInfo? LoadPlayerInfo(string playerName)
    {
        using var conn = new NpgsqlConnection(ConnString);
        int? id = conn.Query<int?>("select player_id from player_nick where player_nick = @name order by last_used desc", new { name = playerName }).FirstOrDefault();
        if (id == null)
        {
            return null;
        }

        var aliases = conn.Query<PlayerAliasInfo>(
                "select player_nick as Alias, last_used as LastUsed from player_nick where player_id=@id",
                new { id = id })
            .AsList();

        var bans = conn.Query<BanInfo>(
                @"select 
                    ban.id as Id,
                    player_id as PlayerId,
                    @pname as PlayerName,
                    reason,
                    created as StartTime,
                    expires as ExpiryTime,
                    ad.name as AdminName
                from ban 
                    inner join admin ad on ban.admin_id = ad.id
                where player_id=@id
                    ",
                new { id = id, pname = playerName })
                .AsList();

        return new PlayerInfo
        {
            Id = id.Value,
            Aliases = aliases.OrderByDescending(a => a.LastUsed).ToArray(),
            Bans = bans.OrderByDescending(b => b.ExpiryTime).ToArray()
        };
    }

    public void AddNewBan(int adminId, int playerId, string reason, int durationMinutes)
    {
        using var conn = new NpgsqlConnection(ConnString);
        conn.Execute("insert into ban(player_id, reason, show_reason, admin_id, created, last_updated, expires) values (@playerId, @reason, true, @adminId, @created, @created, @expires);",
        new
        {
            playerId = playerId,
            adminId = adminId,
            reason = reason,
            created = DateTime.Now,
            expires = DateTime.Now.AddMinutes(durationMinutes)
        });
    }

    public BanInfo? GetBanInfo(int banId)
    {
        using var conn = new NpgsqlConnection(ConnString);

        var ban = conn.Query<BanInfo>(
            @"select 
                    ban.id as Id,
                    player_id as PlayerId,
                    '' as PlayerName,
                    reason,
                    created as StartTime,
                    expires as ExpiryTime,
                    ad.name as AdminName
                from ban 
                    inner join admin ad on ban.admin_id = ad.id
                where ban.id = @id
                    ",
            new { id = banId }).FirstOrDefault();

        if (ban != null)
        {
            ban.PlayerName = DoGetLastPlayerName(ban.PlayerId, conn) ?? "<unknown>";
        }

        return ban;
    }

    public void ChangeBanDuration(int banId, int newDurationMinutes, int adminId)
    {
        using var conn = new NpgsqlConnection(ConnString);
        var newExpiry = DateTime.Now.AddMinutes(newDurationMinutes);
        conn.Execute("update ban set expires=@expires, last_updated=@now where id=@id",
        new
        {
            id = banId,
            expires = newExpiry,
            now = DateTime.Now
        });
    }
}