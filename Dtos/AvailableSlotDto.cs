namespace MeetSlot.Dtos
{
    public class AvailableSlotDto  // brukes for å returnere ledige tider
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    
    }
    
}