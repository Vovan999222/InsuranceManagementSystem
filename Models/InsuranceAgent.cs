namespace InsuranceManagementSystem.Models
{
    public class InsuranceAgent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal CommissionRate { get; set; }
        public DateTime HireDate { get; set; } = DateTime.Now;
    }
}