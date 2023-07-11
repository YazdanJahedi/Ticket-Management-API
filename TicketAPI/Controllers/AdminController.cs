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
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("tickets")]
        public ActionResult<Ticket> GetTickets(bool isCheckd = false)
        {
            if (_context.Tickets == null)
            {
                return NotFound();
            }

            var items = _context.Tickets.Where(a => a.IsChecked == isCheckd).ToList();

            return Ok(items);
        }


        [HttpPost("tickets/{ticketId}")]
        public async Task<ActionResult<Ticket>> PostNewResponse(long ticketId, ResponseDto req)
        {
            if (_context.Responses == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = _context.Tickets.Find(ticketId);

            if (ticket == null)
            {
                return BadRequest("ticketId not found");
            }

            // upate number of responses and isChecked fields
            ticket.NumberOfResponses++;
            ticket.IsChecked = true;
            if (ticket.FirstResponseDate == null)
            {
                ticket.FirstResponseDate = DateTime.Now;
            }

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

    }
}
