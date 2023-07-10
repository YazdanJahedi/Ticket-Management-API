using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketAPI.Models;

namespace TicketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("request/FAQ")]
        public async Task<ActionResult<IEnumerable<FAQTitle>>> GetFAQTitles()
        {
            if (_context.FAQTitles == null)
            {
                return NotFound();
            }
            return await _context.FAQTitles.ToListAsync();
        }

        [HttpGet("request/FAQ/{id}")]
        public ActionResult<IEnumerable<FAQItem>> GetFAQItems(int id)
        {
            if (_context.FAQItems == null)
            {
                return NotFound();
            }

            var items = _context.FAQItems.Where(a => a.TitleId == id).ToList();

            return Ok(items);
        }


        [HttpPost("request/tickets")]
        public async Task<ActionResult<Ticket>> PostTodoItem(TicketDto req)
        {
            if (_context.Tickets == null)
            {
                return NotFound();
            }

            var id = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;
            var userId = Convert.ToInt64(id);

            var ticket = new Ticket
            {
                UserId = userId,
                Title = req.Title,
                Description = req.Description,
                IsChecked = false,
                CreateDate = DateTime.Now,
                FirstResponseDate = null,
                CloseDate = null,
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(ticket);
        }


        [HttpGet("request/tickets")]
        public ActionResult<Ticket> GetTodoItems()
        {
            if (_context.Tickets == null)
            {
                return NotFound();
            }

            var id = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;
            var userId = Convert.ToInt64(id);

            var items = _context.Tickets.Where(a => a.UserId == userId).ToList();
            return Ok(items);
        }

    }
}
