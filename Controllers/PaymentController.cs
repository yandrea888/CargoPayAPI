using Microsoft.AspNetCore.Mvc;
using CargoPayAPI.Data;
using CargoPayAPI.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CargoPayAPI.Services;
using System.Transactions;

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
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
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

                // Aplicar la tarifa de servicio
                decimal fee = FeeService.Instance.GetCurrentFee();
                decimal totalAmount = payment.Amount + fee;

                if (card.Balance < totalAmount)
                {
                    return BadRequest("Insufficient balance including fee.");
                }

                // Descontar saldo y guardar el pago
                card.Balance -= totalAmount;
                payment.Fee = fee;

                _context.Payments.Add(payment);
                _context.SaveChanges();

                transaction.Complete(); 
            }

            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, new
            {
                Message = "Payment successful",
                PaymentId = payment.Id,
                Amount = payment.Amount,
                Fee = payment.Fee,
                TotalCharged = payment.Amount + payment.Fee,
                RemainingBalance = _context.Cards.First(c => c.Id == payment.CardId).Balance
            });
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
                .Include(p => p.Card) 
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
                .Include(p => p.Card) 
                .Where(p => p.CardId == cardId)
                .ToList();

            return Ok(payments);
        }
    }
}
