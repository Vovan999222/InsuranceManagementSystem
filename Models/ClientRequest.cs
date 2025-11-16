namespace InsuranceManagementSystem.Models
{
    public enum RequestStatus
    {
        New,
        Processed,
        Completed,
        Rejected
    }

    public class ClientRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ClientId { get; set; } = string.Empty;
        public PolicyType RequestedType { get; set; }
        public decimal DesiredCoverageAmount { get; set; }
        public int DurationMonths { get; set; }
        public string AdditionalInfo { get; set; } = string.Empty;
        public RequestStatus Status { get; set; } = RequestStatus.New;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ProcessedDate { get; set; }
    }
}