namespace InsuranceManagementSystem.Models
{
    public enum PolicyType
    {
        Auto,
        Health,
        Property
    }

    public enum PolicyStatus
    {
        Active,
        Suspended,
        Completed,
        Expired
    }

    public class InsurancePolicy
    {
        public string PolicyNumber { get; set; } = string.Empty;
        public PolicyType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal CoverageAmount { get; set; }
        public decimal Premium { get; set; }
        public PolicyStatus Status { get; set; } = PolicyStatus.Active;
        public string ClientId { get; set; } = string.Empty;
        public string AgentId { get; set; } = string.Empty;

        public bool IsExpired => DateTime.Today > EndDate;

        public void UpdateStatus()
        {
            if (IsExpired && Status == PolicyStatus.Active)
            {
                Status = PolicyStatus.Expired;
            }
        }
    }
}