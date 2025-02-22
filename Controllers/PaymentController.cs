using Microsoft.AspNetCore.Mvc;
using CargoPayAPI.Data;
using CargoPayAPI.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CargoPayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/payment
        [HttpPost]
        public IActionResult CreatePayment([FromBody] Payment payment)
        {
            var card = _context.Cards.FirstOrDefault(c => c.Id == payment.CardId);
            if (card == null)
            {
                return NotFound("Card not found.");
            }

            if (card.Balance < payment.Amount)
            {
                return BadRequest("Insufficient balance.");
            }

            // Deduct the amount from the card balance
            card.Balance -= payment.Amount;

            _context.Payments.Add(payment);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
        }

        // GET: api/payment
        [HttpGet]
        public IActionResult GetAllPayments()
        {
            var payments = _context.Payments
            .Include(p => p.Card) 
            .ToList();

            return Ok(payments);
        }

        // GET: api/payment/{id}
        [HttpGet("{id}")]
        public IActionResult GetPaymentById(Guid id)
        {
            var payment = _context.Payments
                .Include(p => p.Card) // Incluir la tarjeta
                .FirstOrDefault(p => p.Id == id);

            if (payment == null)
            {
                return NotFound("Payment not found.");
            }

            return Ok(payment);
        }

        // GET: api/payment/card/{cardId}
        [HttpGet("card/{cardId}")]
        public IActionResult GetPaymentsByCard(Guid cardId)
        {
            var payments = _context.Payments
                .Include(p => p.Card) // Incluir la tarjeta
                .Where(p => p.CardId == cardId)
                .ToList();

            return Ok(payments);
        }
    }
}
