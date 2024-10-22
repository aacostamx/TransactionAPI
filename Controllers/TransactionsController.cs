using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionAPI.Data;
using TransactionAPI.Models;

namespace TransactionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(string? searchQuery)
        {
            var query = _context.Transactions.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(t =>
                    t.TransactionId.Contains(searchQuery) ||
                    t.Status.Contains(searchQuery));
            }

            query = query.OrderByDescending(t => t.CreatedDate);

            var transactions = await query.Take(500).ToListAsync();

            return transactions;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }

            transaction.Date = transaction.Date.ToUniversalTime();
            transaction.CreatedDate = transaction.CreatedDate.ToUniversalTime();

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return NoContent();
        }



        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction([FromBody] TransactionDto transactionDto)
        {
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                Date = DateTime.SpecifyKind(transactionDto.Date, DateTimeKind.Utc),
                Amount = transactionDto.Amount,
                Status = transactionDto.Status,
                CreatedDate = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }



        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
