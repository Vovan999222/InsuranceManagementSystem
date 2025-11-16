namespace InsuranceManagementSystem.Models
{
    public enum PaymentType
    {
        Premium,    // Внесок
        Payout      // Виплата
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }

    public class Payment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public PaymentType Type { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string PolicyNumber { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}