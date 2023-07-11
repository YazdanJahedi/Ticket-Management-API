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
                NumberOfResponses = 0,
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok(ticket);
        }


        [HttpGet("request/tickets")]
        public ActionResult<Ticket> GetTickes()
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


        [HttpPost("request/tickets/{ticketId}")]
        public async Task<ActionResult<Ticket>> PostNewResponse(long ticketId, ResponseDto req)
        {
            if (_context.Responses == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var userIdString = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;
            var userId = Convert.ToInt64(userIdString);
            var ticket = _context.Tickets.Find(ticketId);

            if (ticket == null)
            {
                return BadRequest("ticketId not found");
            }
            if (ticket.UserId != userId)
            {
                return BadRequest("You do not have access to entered ticket");
            }

            // upate number of responses and isChecked fields
            ticket.NumberOfResponses++;
            ticket.IsChecked = false;

            var userEmail = User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault()?.Value;

            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            var response = new Response
            {
                TicketId = ticketId,
                IdInTicket = ticket.NumberOfResponses,
                Writer = userEmail,
                Text = req.Text,
                SentDate = DateTime.Now,
            };

            _context.Responses.Add(response);
            await _context.SaveChangesAsync();

            return Ok(response);
        }

        [HttpGet("responses/{ticketId}")]
        public ActionResult<Ticket> GetResponses(long ticketId)
        {
            if (_context.Responses == null)
            {
                return NotFound();
            }

            var items = _context.Responses.Where(a => a.TicketId == ticketId).ToList();

            return Ok(items);
        }
    }
}
