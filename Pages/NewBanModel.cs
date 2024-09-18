using CbAdmin;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class NewBanModel : PageModel
{
    private DataClient dataClient;

    public string PlayerName { get; set; } = null!;
    public int PlayerId { get; set; }

    public BanPresetInfo[] Presets { get; set; } = null!;

    public NewBanModel(IConfiguration configuration)
    {
        dataClient = new DataClient(configuration);
    }

    public void OnGet(int playerId)
    {
        var admin = dataClient.GetAdminInfo(User.Identity?.Name);
        if (admin == null)
        {
            throw new UnauthorizedAccessException();
        }

        var name = dataClient.GetLastPlayerName(playerId);
        if (name == null)
        {
            throw new KeyNotFoundException();
        }

        PlayerId = playerId;
        PlayerName = name;
        Presets = dataClient.LoadBanPresets();
    }
}