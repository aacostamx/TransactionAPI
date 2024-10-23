using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TransactionAPI.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? TransactionId { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? Description { get; set; }
        public string? OriginAccount { get; set; }
        public string? DestinationAccount { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Currency { get; set; }
        public bool? IsActive { get; set; }
        public string? TransactionType { get; set; }
        public bool? IsRecurring { get; set; }
    }
}
