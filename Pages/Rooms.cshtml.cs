using MeetSlot.Data;
using MeetSlot.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MeetSlot.Pages;

public class RoomsModel : PageModel
{
    private readonly MeetSlotDbContext _context;

    public RoomsModel(MeetSlotDbContext context)
    {
        _context = context;
    }

    public List<MeetingRoom> Rooms { get; private set; } = new();

    // Henter rom alfabetisk slik at visningen blir stabil og lett å lese.
    public async Task OnGetAsync()
    {
        Rooms = await _context.MeetingRooms
            .OrderBy(r => r.Name)
            .ToListAsync();
    }
}
