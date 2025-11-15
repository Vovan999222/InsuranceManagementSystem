using InsuranceManagementSystem.Models;

namespace InsuranceManagementSystem.Services
{
    public class InsuranceService
    {
        private List<Client> _clients;
        private List<InsurancePolicy> _policies;
        private List<InsuranceAgent> _agents;
        private List<InsuranceClaim> _claims;
        private List<ClientRequest> _requests;
        private List<Payment> _payments;
        private readonly DataService _dataService;

        public InsuranceService(DataService dataService)
        {
            _dataService = dataService;
            LoadData();
        }

        private void LoadData()
        {
            _clients = _dataService.LoadClients();
            _policies = _dataService.LoadPolicies();
            _agents = _dataService.LoadAgents();
            _claims = _dataService.LoadClaims();
            _requests = _dataService.LoadRequests();
            _payments = _dataService.LoadPayments();
        }

        private void SaveAllData()
        {
            _dataService.SaveClients(_clients);
            _dataService.SavePolicies(_policies);
            _dataService.SaveAgents(_agents);
            _dataService.SaveClaims(_claims);
            _dataService.SaveRequests(_requests);
            _dataService.SavePayments(_payments);
        }

        // CRUD для клієнтів
        public void AddClient(Client client)
        {
            _clients.Add(client);
            SaveAllData();
        }

        public List<Client> GetAllClients() => _clients;

        public Client GetClient(string id) => _clients.FirstOrDefault(c => c.Id == id);

        public void UpdateClient(Client updatedClient)
        {
            var client = _clients.FirstOrDefault(c => c.Id == updatedClient.Id);
            if (client != null)
            {
                client.FullName = updatedClient.FullName;
                client.Email = updatedClient.Email;
                client.Phone = updatedClient.Phone;
                client.Address = updatedClient.Address;
                client.Type = updatedClient.Type;
                SaveAllData();
            }
        }

        public void DeleteClient(string id)
        {
            var client = _clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                _clients.Remove(client);
                SaveAllData();
            }
        }

        // CRUD для полісів
        public void AddPolicy(InsurancePolicy policy)
        {
            policy.PolicyNumber = GeneratePolicyNumber(policy.Type);
            policy.Premium = CalculatePremium(policy.CoverageAmount, policy.Type);
            _policies.Add(policy);

            // Автоматично створюємо платіж за преміум
            CreatePremiumPayment(policy);

            SaveAllData();
        }

        public List<InsurancePolicy> GetAllPolicies() => _policies;

        public InsurancePolicy GetPolicy(string policyNumber) =>
            _policies.FirstOrDefault(p => p.PolicyNumber == policyNumber);

        private string GeneratePolicyNumber(PolicyType type)
        {
            var prefix = type switch
            {
                PolicyType.Auto => "AUTO",
                PolicyType.Health => "HLTH",
                PolicyType.Property => "PROP",
                _ => "GEN"
            };
            return $"{prefix}-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        private decimal CalculatePremium(decimal coverageAmount, PolicyType type)
        {
            decimal rate = type switch
            {
                PolicyType.Auto => 0.05m,
                PolicyType.Health => 0.03m,
                PolicyType.Property => 0.04m,
                _ => 0.05m
            };
            return coverageAmount * rate;
        }

        // Робота з claims
        public void AddClaim(InsuranceClaim claim)
        {
            _claims.Add(claim);
            SaveAllData();
        }

        public List<InsuranceClaim> GetAllClaims() => _claims;

        public List<InsuranceClaim> GetClaimsByPolicy(string policyNumber) =>
            _claims.Where(c => c.PolicyNumber == policyNumber).ToList();

        // CRUD для агентів
        public void AddAgent(InsuranceAgent agent)
        {
            _agents.Add(agent);
            SaveAllData();
        }

        public List<InsuranceAgent> GetAllAgents() => _agents;

        // Етап 2: Робота з статусами
        public void UpdatePolicyStatus(string policyNumber, PolicyStatus newStatus)
        {
            var policy = _policies.FirstOrDefault(p => p.PolicyNumber == policyNumber);
            if (policy != null)
            {
                policy.Status = newStatus;
                SaveAllData();
            }
        }

        public void UpdateClaimStatus(string claimId, ClaimStatus newStatus)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                claim.Status = newStatus;

                // Автоматично створюємо виплату при затвердженні claim
                if (newStatus == ClaimStatus.Approved)
                {
                    CreatePayoutPayment(claim);
                }

                SaveAllData();
            }
        }

        // Пошук полісів за різними критеріями
        public List<InsurancePolicy> SearchPolicies(
            PolicyType? type = null,
            string clientId = null,
            PolicyStatus? status = null,
            decimal? minPremium = null,
            decimal? maxPremium = null)
        {
            var query = _policies.AsEnumerable();

            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);

            if (!string.IsNullOrEmpty(clientId))
                query = query.Where(p => p.ClientId == clientId);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            if (minPremium.HasValue)
                query = query.Where(p => p.Premium >= minPremium.Value);

            if (maxPremium.HasValue)
                query = query.Where(p => p.Premium <= maxPremium.Value);

            return query.ToList();
        }

        // Автоматичне оновлення статусів полісів (закінчення терміну)
        public void UpdateExpiredPolicies()
        {
            var today = DateTime.Today;
            foreach (var policy in _policies)
            {
                if (policy.EndDate < today && policy.Status == PolicyStatus.Active)
                {
                    policy.Status = PolicyStatus.Expired;
                }
            }
            SaveAllData();
        }

        // Отримати статистику клієнта
        public (int PoliciesCount, decimal TotalPayouts) GetClientStats(string clientId)
        {
            var clientPolicies = _policies.Where(p => p.ClientId == clientId).ToList();
            var policiesCount = clientPolicies.Count;

            var totalPayouts = _claims
                .Where(c => c.ClientId == clientId && (c.Status == ClaimStatus.Paid || c.Status == ClaimStatus.Approved))
                .Sum(c => c.ClaimAmount);

            return (policiesCount, totalPayouts);
        }

        // Етап 3: Робота з запитами клієнтів
        public void AddClientRequest(ClientRequest request)
        {
            _requests.Add(request);
            SaveAllData();
        }

        public List<ClientRequest> GetClientRequests(string clientId = null)
        {
            if (string.IsNullOrEmpty(clientId))
                return _requests;

            return _requests.Where(r => r.ClientId == clientId).ToList();
        }

        public void UpdateRequestStatus(string requestId, RequestStatus status)
        {
            var request = _requests.FirstOrDefault(r => r.Id == requestId);
            if (request != null)
            {
                request.Status = status;
                request.ProcessedDate = DateTime.Now;
                SaveAllData();
            }
        }

        // Метод для підбору полісів під запит
        public List<InsurancePolicy> FindMatchingPolicies(ClientRequest request)
        {
            return _policies
                .Where(p => p.Type == request.RequestedType &&
                           p.CoverageAmount >= request.DesiredCoverageAmount * 0.8m &&
                           p.CoverageAmount <= request.DesiredCoverageAmount * 1.2m &&
                           p.Status == PolicyStatus.Active)
                .ToList();
        }

        // Етап 4: Робота з платежами
        public void AddPayment(Payment payment)
        {
            _payments.Add(payment);
            SaveAllData();
        }

        public List<Payment> GetPayments(string policyNumber = null, string clientId = null)
        {
            var query = _payments.AsEnumerable();

            if (!string.IsNullOrEmpty(policyNumber))
                query = query.Where(p => p.PolicyNumber == policyNumber);

            if (!string.IsNullOrEmpty(clientId))
                query = query.Where(p => p.ClientId == clientId);

            return query.ToList();
        }

        public void UpdatePaymentStatus(string paymentId, PaymentStatus status)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment != null)
            {
                payment.Status = status;
                SaveAllData();
            }
        }

        // Метод для автоматичного створення платежу при створенні поліса
        public void CreatePremiumPayment(InsurancePolicy policy)
        {
            var payment = new Payment
            {
                Amount = policy.Premium,
                Type = PaymentType.Premium,
                Status = PaymentStatus.Pending,
                PolicyNumber = policy.PolicyNumber,
                ClientId = policy.ClientId,
                Description = $"Premium payment for policy {policy.PolicyNumber}"
            };

            AddPayment(payment);
        }

        // Метод для автоматичного створення виплати при затвердженні claim
        public void CreatePayoutPayment(InsuranceClaim claim)
        {
            var policy = GetPolicy(claim.PolicyNumber);
            if (policy != null)
            {
                var payment = new Payment
                {
                    Amount = claim.ClaimAmount,
                    Type = PaymentType.Payout,
                    Status = PaymentStatus.Pending,
                    PolicyNumber = claim.PolicyNumber,
                    ClientId = claim.ClientId,
                    Description = $"Payout for claim {claim.Id}: {claim.Description}"
                };

                AddPayment(payment);
            }
        }

        // Статистика по типах полісів
        public Dictionary<PolicyType, (int Active, int Completed, int Expired, decimal AveragePremium)> GetPolicyTypeStatistics()
        {
            var policies = _policies;
            var result = new Dictionary<PolicyType, (int, int, int, decimal)>();

            foreach (PolicyType type in Enum.GetValues(typeof(PolicyType)))
            {
                var typePolicies = policies.Where(p => p.Type == type).ToList();
                var active = typePolicies.Count(p => p.Status == PolicyStatus.Active);
                var completed = typePolicies.Count(p => p.Status == PolicyStatus.Completed);
                var expired = typePolicies.Count(p => p.Status == PolicyStatus.Expired);
                var averagePremium = typePolicies.Any() ? typePolicies.Average(p => p.Premium) : 0;

                result[type] = (active, completed, expired, averagePremium);
            }

            return result;
        }
    }
}