namespace MeetSlot.Dtos
{
    public class CreateBookingDto  // brukes når en ny booking opprettes
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MeetingRoomId { get; set; }
        public int AppUserId { get; set; }
    }
}

