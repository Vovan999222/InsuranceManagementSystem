namespace InsuranceManagementSystem.Models
{
    public enum UserRole
    {
        Client,
        Agent,
        Manager
    }

    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string AssociatedClientId { get; set; } = string.Empty;
        public string AssociatedAgentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}