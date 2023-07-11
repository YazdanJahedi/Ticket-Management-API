namespace TicketAPI.Models
{
    public class Response
    {
        public long Id { get; set; }
        public long TicketId { get; set; }
        public long IdInTicket { get; set; }
        public string? Writer { get; set; }
        public string? Text { get; set; }
        public DateTime? SentDate { get; set; }
    }
}
