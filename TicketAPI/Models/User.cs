namespace TicketAPI.Models
{
    public class User
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }

        // Tickets []

        public override string ToString()
        {
            return "{" +
                "\n  Id: " + this.Id +
                "\n  Name: " + this.Name +
                "\n  Email: " + this.Email +
                "\n  Role: " + this.Role +
                "\n  PhoneNumer: " + this.PhoneNumber +
                "\n}";
        }
    }
}

