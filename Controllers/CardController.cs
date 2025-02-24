using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CargoPayAPI.Data;
using CargoPayAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CargoPayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/card
        [Authorize]
        [HttpPost]
        public IActionResult CreateCard([FromBody] Card card)
        {
            if (string.IsNullOrWhiteSpace(card.CardNumber) || card.CardNumber.Length != 15 || !card.CardNumber.All(char.IsDigit))
            {
                return BadRequest("Card number must be exactly 15 digits and contain only numbers.");
            }

            try
            {
                _context.Cards.Add(card);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetCardBalance), new { cardNumber = card.CardNumber }, card);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true)
            {
                return BadRequest("Card number already exists.");
            }
        }

        // GET: api/card/{cardNumber}
        [Authorize]
        [HttpGet("{cardNumber}")]
        public IActionResult GetCardBalance(string cardNumber)
        {
            var card = _context.Cards.AsNoTracking().FirstOrDefault(c => c.CardNumber == cardNumber);
            if (card == null)
            {
                return NotFound("Card not found.");
            }

            return Ok(new { 
                cardNumber = card.CardNumber,
                Balance = card.Balance 
            });
        }
    }
}
