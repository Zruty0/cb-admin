using Npgsql;
using NpgsqlTypes;
using Dapper;
using System.Runtime.CompilerServices;

namespace CbAdmin;

public class DataClient
{
    public class AdminInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private string ConnString;

    public DataClient(IConfiguration configuration)
    {
        ConnString = configuration.GetValue<string>("DatabaseConnString")!;
    }

    public AdminInfo? GetAdminInfo(string? userName)
    {
        using var conn = new NpgsqlConnection(ConnString);
        var admin = conn.Query<AdminInfo>("SELECT * FROM admin WHERE LOWER(name)=@name", new { name = userName }).AsList().SingleOrDefault();
        return admin;
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
}