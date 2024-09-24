using CbAdmin;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RecentBansModel : PageModel
{
    private DataClient dataClient;

    public BanInfo[] RecentBans { get; set; } = null!;

    public RecentBansModel(IConfiguration configuration)
    {
        dataClient = new DataClient(configuration);
    }

    public void OnGet(int n=20)
    {
        if (dataClient.GetAdminInfo(this.User) == null)
        {
            throw new UnauthorizedAccessException();
        }
        RecentBans = dataClient.GetRecentBans(n);
    }
}