using System.Text.Json.Serialization;

namespace InsuranceManagementSystem.Models
{
    public enum ClientType
    {
        Individual,
        Corporate
    }

    public class Client
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public ClientType Type { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public List<InsurancePolicy> Policies { get; set; } = new List<InsurancePolicy>();

        [JsonIgnore]
        public int ActivePoliciesCount => Policies?.Count(p => p.Status == PolicyStatus.Active) ?? 0;

        [JsonIgnore]
        public decimal TotalCoverageAmount => Policies?.Where(p => p.Status == PolicyStatus.Active).Sum(p => p.CoverageAmount) ?? 0;
    }
}