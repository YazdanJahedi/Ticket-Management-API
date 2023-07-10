namespace TicketAPI.Models
{
    public class Ticket
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsChecked { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? FirstResponseDate { get; set; }
        public DateTime? CloseDate { get; set; }

    }
}
