using CbAdmin;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class AdminModel : PageModel
{
    private DataClient dataClient;

    public DataClient.AdminInfo AdminInfo { get; private set; } = null!;

    public AdminModel(IConfiguration configuration)
    {
        dataClient = new DataClient(configuration);
    }

    public void OnGet()
    {
        var admin = dataClient.GetAdminInfo(User.Identity?.Name);
        if (admin == null)
        {
            throw new UnauthorizedAccessException();
        }
        AdminInfo = admin;
    }
}
