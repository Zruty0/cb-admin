using CbAdmin;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class UserBansModel : PageModel
{
    private DataClient dataClient;

    public string PlayerName { get; set; } = null!;

    public UserBansModel(IConfiguration configuration)
    {
        dataClient = new DataClient(configuration);
    }

    public void OnGet(string? name)
    {
        var admin = dataClient.GetAdminInfo(User);
        if (admin == null)
        {
            throw new UnauthorizedAccessException();
        }
        PlayerName = name ?? "";
    }
}