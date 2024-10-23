namespace TransactionAPI.Models
{
    public class TransactionDto
    {
        public string? TransactionId { get; set; }
        public DateTime Date { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
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
