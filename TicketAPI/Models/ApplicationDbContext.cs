using Microsoft.EntityFrameworkCore;

namespace TicketAPI.Models
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Ticket> Tickets { get; set; } = null!;
        public DbSet<FAQTitle> FAQTitles { get; set; } = null!;
        public DbSet<FAQItem> FAQItems { get; set; } = null!;
        
    }
}
