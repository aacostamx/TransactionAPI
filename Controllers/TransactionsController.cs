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
                searchQuery = searchQuery.ToLower();

                // Check if the searchQuery is numeric for amount comparison
                decimal searchAmount;
                bool isNumeric = decimal.TryParse(searchQuery, out searchAmount);

                query = query.Where(t =>
                    (t.TransactionId != null && t.TransactionId.ToLower().Contains(searchQuery)) ||  // Case-insensitive search on TransactionId
                    (t.Status != null && t.Status.ToLower().Contains(searchQuery)) ||
                    (t.Description != null && t.Description.ToLower().Contains(searchQuery)) ||
                    (t.OriginAccount != null && t.OriginAccount.ToLower().Contains(searchQuery)) ||
                    (t.DestinationAccount != null && t.DestinationAccount.ToLower().Contains(searchQuery)) ||
                    (t.Category != null && t.Category.ToLower().Contains(searchQuery)) ||
                    (t.Type != null && t.Type.ToLower().Contains(searchQuery)) ||
                    (t.Currency != null && t.Currency.ToLower().Contains(searchQuery)) ||
                    (isNumeric && t.Amount != null && t.Amount == searchAmount));   // Only check Amount if searchQuery is numeric
            }

            // Order by CreatedDate and take the top 500 results
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
            transaction.CreatedDate = transaction.CreatedDate?.ToUniversalTime();

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
                Amount = transactionDto.Amount,
                Status = transactionDto.Status,
                Date = DateTime.SpecifyKind(transactionDto.Date, DateTimeKind.Utc),
                CreatedDate = DateTime.UtcNow,
                Description = transactionDto.Description,
                OriginAccount = transactionDto.OriginAccount,
                DestinationAccount = transactionDto.DestinationAccount,
                Category = transactionDto.Category,
                Type = transactionDto.Type,
                Currency = transactionDto.Currency,
                IsActive = transactionDto.IsActive,
                TransactionType = transactionDto.TransactionType,
                IsRecurring = transactionDto.IsRecurring
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
