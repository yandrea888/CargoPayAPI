using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CargoPayAPI.Data;
using CargoPayAPI.Models;

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
        [HttpPost]
        public IActionResult CreateCard([FromBody] Card card)
        {
            if (_context.Cards.Any(c => c.CardNumber == card.CardNumber))
            {
                return BadRequest("Card number already exists.");
            }

            _context.Cards.Add(card);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetCardBalance), new { cardNumber = card.CardNumber }, card);
        }

        // GET: api/card/{cardNumber}
        [HttpGet("{cardNumber}")]
        public IActionResult GetCardBalance(string cardNumber)
        {
            var card = _context.Cards.FirstOrDefault(c => c.CardNumber == cardNumber);
            if (card == null)
            {
                return NotFound("Card not found.");
            }

            return Ok(new { Balance = card.Balance });
        }
    }
}
