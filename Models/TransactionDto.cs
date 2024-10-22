namespace TransactionAPI.Models
{
    public class TransactionDto
    {
        public string TransactionId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
