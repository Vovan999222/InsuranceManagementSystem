using InsuranceManagementSystem.Models;

namespace InsuranceManagementSystem.Services
{
    public class StatisticsService
    {
        private readonly InsuranceService _insuranceService;

        public StatisticsService(InsuranceService insuranceService)
        {
            _insuranceService = insuranceService;
        }

        public int GetActivePoliciesCount()
        {
            var policies = _insuranceService.GetAllPolicies();
            return policies.Count(p => p.Status == PolicyStatus.Active);
        }

        public int GetClaimsCount(DateTime? startDate = null, DateTime? endDate = null)
        {
            var claims = _insuranceService.GetAllClaims();
            var query = claims.AsEnumerable();

            if (startDate.HasValue)
                query = query.Where(c => c.ClaimDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(c => c.ClaimDate <= endDate.Value);

            return query.Count();
        }

        public decimal GetTotalPayouts()
        {
            var claims = _insuranceService.GetAllClaims();
            return claims
                .Where(c => c.Status == ClaimStatus.Paid)
                .Sum(c => c.ClaimAmount);
        }

        public decimal GetCompanyRevenue()
        {
            var policies = _insuranceService.GetAllPolicies();
            var totalPremiums = policies.Sum(p => p.Premium);
            var totalPayouts = GetTotalPayouts();

            return totalPremiums - totalPayouts;
        }

        public Dictionary<PolicyType, (int Count, decimal TotalPremium)> GetPolicyTypeStatistics()
        {
            var policies = _insuranceService.GetAllPolicies();
            var result = new Dictionary<PolicyType, (int Count, decimal TotalPremium)>();

            foreach (PolicyType type in Enum.GetValues(typeof(PolicyType)))
            {
                var typePolicies = policies.Where(p => p.Type == type).ToList();
                result[type] = (typePolicies.Count, typePolicies.Sum(p => p.Premium));
            }

            return result;
        }

        public List<(InsuranceAgent Agent, int PoliciesSold, decimal TotalCommission)> GetAgentPerformance()
        {
            var agents = _insuranceService.GetAllAgents();
            var policies = _insuranceService.GetAllPolicies();
            var result = new List<(InsuranceAgent, int, decimal)>();

            foreach (var agent in agents)
            {
                var agentPolicies = policies.Where(p => p.AgentId == agent.Id).ToList();
                var policiesSold = agentPolicies.Count;
                var totalCommission = agentPolicies.Sum(p => p.Premium * (agent.CommissionRate / 100m));

                result.Add((agent, policiesSold, totalCommission));
            }

            return result.OrderByDescending(x => x.Item3).ToList();
        }

        public Dictionary<string, int> GetClaimsByStatus()
        {
            var claims = _insuranceService.GetAllClaims();
            return claims
                .GroupBy(c => c.Status)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());
        }

        public Dictionary<PolicyType, (int Active, int Completed, int Expired, decimal AveragePremium)> GetDetailedPolicyStatistics()
        {
            return _insuranceService.GetPolicyTypeStatistics();
        }

        public (decimal TotalPremiums, decimal TotalPayouts, decimal Balance) GetFinancialSummary()
        {
            var payments = _insuranceService.GetPayments();
            var totalPremiums = payments.Where(p => p.Type == PaymentType.Premium && p.Status == PaymentStatus.Completed)
                               .Sum(p => p.Amount);
            var totalPayouts = payments.Where(p => p.Type == PaymentType.Payout && p.Status == PaymentStatus.Completed)
                              .Sum(p => p.Amount);
            var balance = totalPremiums - totalPayouts;

            return (totalPremiums, totalPayouts, balance);
        }

        public List<(string Month, decimal Premiums, decimal Payouts)> GetMonthlyFinancials(int months = 12)
        {
            var payments = _insuranceService.GetPayments();
            var endDate = DateTime.Now;
            var startDate = endDate.AddMonths(-months);

            var monthlyData = payments
                .Where(p => p.PaymentDate >= startDate)
                .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                .Select(g => (
                    Month: $"{g.Key.Year}-{g.Key.Month:D2}",
                    Premiums: g.Where(p => p.Type == PaymentType.Premium && p.Status == PaymentStatus.Completed).Sum(p => p.Amount),
                    Payouts: g.Where(p => p.Type == PaymentType.Payout && p.Status == PaymentStatus.Completed).Sum(p => p.Amount)
                ))
                .OrderBy(x => x.Month)
                .ToList();

            return monthlyData;
        }
    }
}