namespace InsuranceManagementSystem.Models
{
    public enum ClaimStatus
    {
        New,
        InReview,
        Approved,
        Paid,
        Rejected
    }

    public class InsuranceClaim
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime ClaimDate { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public decimal ClaimAmount { get; set; }
        public ClaimStatus Status { get; set; } = ClaimStatus.New;
        public string PolicyNumber { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
    }
}