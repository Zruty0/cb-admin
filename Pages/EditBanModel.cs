using CbAdmin;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class EditBanModel : PageModel
{
    public int BanId { get; set; }
    public BanInfo? BanInfo { get; set; }
    private DataClient dataClient;

    public EditBanModel(IConfiguration configuration)
    {
        dataClient = new DataClient(configuration);
    }

    public void OnGet(int banId)
    {
        BanId = banId;
        BanInfo = dataClient.GetBanInfo(banId);
    }
}