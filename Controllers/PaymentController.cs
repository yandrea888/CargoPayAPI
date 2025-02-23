using Microsoft.AspNetCore.Mvc;
using CargoPayAPI.Data;
using CargoPayAPI.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CargoPayAPI.Services;
using System.Transactions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        // POST: api/payment
        [HttpPost]
        public async Task<IActionResult> CreatePaymentAsync([FromBody] Payment payment)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == payment.CardId);
                if (card == null)
                {
                    return NotFound("Card not found.");
                }

                decimal fee = FeeService.Instance.GetCurrentFee();
                decimal totalAmount = payment.Amount + fee;

                if (card.Balance < totalAmount)
                {
                    return BadRequest("Insufficient balance including fee.");
                }

                // Aplicamos concurrencia con EF Core
                card.Balance -= totalAmount;
                payment.Fee = fee;

                _context.Payments.Add(payment);

                try
                {
                    await _context.SaveChangesAsync(); 
                    transaction.Complete();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Conflict("Transaction failed due to concurrent modification.");
                }

                return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
            }
        }

        // GET: api/payment
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _context.Payments
                .AsNoTracking()
                .Include(p => p.Card)
                .ToListAsync(); 

            return Ok(payments);
        }

        // GET: api/payment/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var payment = await _context.Payments
                .AsNoTracking()
                .Include(p => p.Card)
                .FirstOrDefaultAsync(p => p.Id == id); 

            if (payment == null)
            {
                return NotFound("Payment not found.");
            }

            return Ok(payment);
        }

        // GET: api/payment/card/{cardId}
        [Authorize]
        [HttpGet("card/{cardId}")]
        public async Task<IActionResult> GetPaymentsByCard(Guid cardId)
        {
            var payments = await _context.Payments
                .AsNoTracking() 
                .Include(p => p.Card)
                .Where(p => p.CardId == cardId)
                .ToListAsync(); 

            return Ok(payments);
        }
    }
}
