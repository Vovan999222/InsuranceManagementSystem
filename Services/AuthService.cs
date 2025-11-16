using System.Text.Json;
using InsuranceManagementSystem.Models;

namespace InsuranceManagementSystem.Services
{
    public class AuthService
    {
        private List<User> _users;
        private User _currentUser;
        private readonly DataService _dataService;

        public AuthService(DataService dataService)
        {
            _dataService = dataService;
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                _users = _dataService.LoadUsers();
                if (!_users.Any())
                {
                    CreateDefaultUsers();
                }
            }
            catch
            {
                _users = new List<User>();
                CreateDefaultUsers();
            }
        }

        private void CreateDefaultUsers()
        {
            // Тестові користувачі
            _users.Add(new User
            {
                Username = "manager",
                Password = "manager123",
                Role = UserRole.Manager
            });

            _users.Add(new User
            {
                Username = "agent1",
                Password = "agent123",
                Role = UserRole.Agent,
                AssociatedAgentId = Guid.NewGuid().ToString()
            });

            _users.Add(new User
            {
                Username = "client1",
                Password = "client123",
                Role = UserRole.Client,
                AssociatedClientId = Guid.NewGuid().ToString()
            });

            SaveUsers();
        }

        private void SaveUsers()
        {
            _dataService.SaveUsers(_users);
        }

        public bool Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username == username && u.Password == password && u.IsActive);

            if (user != null)
            {
                _currentUser = user;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public User GetCurrentUser() => _currentUser;

        public bool HasAccess(UserRole requiredRole)
        {
            if (_currentUser == null) return false;

            return _currentUser.Role switch
            {
                UserRole.Manager => true,
                UserRole.Agent => requiredRole == UserRole.Agent || requiredRole == UserRole.Client,
                UserRole.Client => requiredRole == UserRole.Client,
                _ => false
            };
        }

        public void RegisterUser(User user)
        {
            if (_users.Any(u => u.Username == user.Username))
                throw new Exception("Користувач з таким іменем вже існує");

            _users.Add(user);
            SaveUsers();
        }

        public void UpdateUser(User updatedUser)
        {
            var user = _users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user != null)
            {
                user.Username = updatedUser.Username;
                user.Password = updatedUser.Password;
                user.Role = updatedUser.Role;
                user.IsActive = updatedUser.IsActive;
                SaveUsers();
            }
        }
    }
}