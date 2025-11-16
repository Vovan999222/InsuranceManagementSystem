using System.Text.Json;
using InsuranceManagementSystem.Models;

namespace InsuranceManagementSystem.Services
{
    public class DataService
    {
        private const string ClientsFile = "clients.json";
        private const string PoliciesFile = "policies.json";
        private const string AgentsFile = "agents.json";
        private const string ClaimsFile = "claims.json";
        private const string RequestsFile = "requests.json";
        private const string PaymentsFile = "payments.json";
        private const string UsersFile = "users.json";

        public DataService()
        {
            EnsureFilesExist();
        }

        private void EnsureFilesExist()
        {
            if (!File.Exists(ClientsFile)) File.WriteAllText(ClientsFile, "[]");
            if (!File.Exists(PoliciesFile)) File.WriteAllText(PoliciesFile, "[]");
            if (!File.Exists(AgentsFile)) File.WriteAllText(AgentsFile, "[]");
            if (!File.Exists(ClaimsFile)) File.WriteAllText(ClaimsFile, "[]");
            if (!File.Exists(RequestsFile)) File.WriteAllText(RequestsFile, "[]");
            if (!File.Exists(PaymentsFile)) File.WriteAllText(PaymentsFile, "[]");
            if (!File.Exists(UsersFile)) File.WriteAllText(UsersFile, "[]");
        }

        public List<Client> LoadClients()
        {
            try
            {
                var json = File.ReadAllText(ClientsFile);
                return JsonSerializer.Deserialize<List<Client>>(json) ?? new List<Client>();
            }
            catch (Exception)
            {
                return new List<Client>();
            }
        }

        public void SaveClients(List<Client> clients)
        {
            var json = JsonSerializer.Serialize(clients, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ClientsFile, json);
        }

        public List<InsurancePolicy> LoadPolicies()
        {
            try
            {
                var json = File.ReadAllText(PoliciesFile);
                return JsonSerializer.Deserialize<List<InsurancePolicy>>(json) ?? new List<InsurancePolicy>();
            }
            catch (Exception)
            {
                return new List<InsurancePolicy>();
            }
        }

        public void SavePolicies(List<InsurancePolicy> policies)
        {
            var json = JsonSerializer.Serialize(policies, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(PoliciesFile, json);
        }

        public List<InsuranceAgent> LoadAgents()
        {
            try
            {
                var json = File.ReadAllText(AgentsFile);
                return JsonSerializer.Deserialize<List<InsuranceAgent>>(json) ?? new List<InsuranceAgent>();
            }
            catch (Exception)
            {
                return new List<InsuranceAgent>();
            }
        }

        public void SaveAgents(List<InsuranceAgent> agents)
        {
            var json = JsonSerializer.Serialize(agents, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(AgentsFile, json);
        }

        public List<InsuranceClaim> LoadClaims()
        {
            try
            {
                var json = File.ReadAllText(ClaimsFile);
                return JsonSerializer.Deserialize<List<InsuranceClaim>>(json) ?? new List<InsuranceClaim>();
            }
            catch (Exception)
            {
                return new List<InsuranceClaim>();
            }
        }

        public void SaveClaims(List<InsuranceClaim> claims)
        {
            var json = JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ClaimsFile, json);
        }

        public List<ClientRequest> LoadRequests()
        {
            try
            {
                var json = File.ReadAllText(RequestsFile);
                return JsonSerializer.Deserialize<List<ClientRequest>>(json) ?? new List<ClientRequest>();
            }
            catch (Exception)
            {
                return new List<ClientRequest>();
            }
        }

        public void SaveRequests(List<ClientRequest> requests)
        {
            var json = JsonSerializer.Serialize(requests, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(RequestsFile, json);
        }

        public List<Payment> LoadPayments()
        {
            try
            {
                var json = File.ReadAllText(PaymentsFile);
                return JsonSerializer.Deserialize<List<Payment>>(json) ?? new List<Payment>();
            }
            catch (Exception)
            {
                return new List<Payment>();
            }
        }

        public void SavePayments(List<Payment> payments)
        {
            var json = JsonSerializer.Serialize(payments, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(PaymentsFile, json);
        }

        public List<User> LoadUsers()
        {
            try
            {
                var json = File.ReadAllText(UsersFile);
                return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        public void SaveUsers(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(UsersFile, json);
        }
    }
}