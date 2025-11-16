using InsuranceManagementSystem.Models;
using InsuranceManagementSystem.Services;

namespace InsuranceManagementSystem
{
    class Program
    {
        private static InsuranceService _insuranceService = null!;
        private static DataService _dataService = null!;
        private static StatisticsService _statisticsService = null!;
        private static AuthService _authService = null!;
        private static ApiIntegrationService _apiIntegrationService = null!;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            InitializeServices();
            ShowLoginMenu();
        }

        static void InitializeServices()
        {
            _dataService = new DataService();
            _insuranceService = new InsuranceService(_dataService);
            _statisticsService = new StatisticsService(_insuranceService);
            _authService = new AuthService(_dataService);
            _apiIntegrationService = new ApiIntegrationService();

            _insuranceService.UpdateExpiredPolicies();
        }

        static void ShowLoginMenu()
        {
            while (true)
            {
                Console.WriteLine("\n= Система управління страховою компанією =");
                Console.WriteLine("1. Увійти");
                Console.WriteLine("2. Зареєструватися");
                Console.WriteLine("3. Вийти");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        if (Login())
                            ShowRoleBasedMenu();
                        break;
                    case "2":
                        Register();
                        break;
                    case "3":
                        Console.WriteLine("До побачення!");
                        return;
                    default:
                        Console.WriteLine("Невірний вибір!");
                        break;
                }
            }
        }

        static bool Login()
        {
            Console.Write("Ім'я користувача: ");
            var username = Console.ReadLine();
            Console.Write("Пароль: ");
            var password = Console.ReadLine();

            if (_authService.Login(username ?? "", password ?? ""))
            {
                var user = _authService.GetCurrentUser();
                Console.WriteLine($"\nВітаємо, {user!.Username}! Роль: {user.Role}");
                return true;
            }
            else
            {
                Console.WriteLine("Невірне ім'я користувача або пароль!");
                return false;
            }
        }

        static void Register()
        {
            Console.WriteLine("\nРеєстрація:");

            var user = new User();
            Console.Write("Ім'я користувача: ");
            user.Username = Console.ReadLine() ?? "";

            if (string.IsNullOrEmpty(user.Username))
            {
                Console.WriteLine("Ім'я користувача не може бути пустим!");
                return;
            }

            Console.Write("Пароль: ");
            user.Password = Console.ReadLine() ?? "";

            if (string.IsNullOrEmpty(user.Password))
            {
                Console.WriteLine("Пароль не може бути пустим!");
                return;
            }

            Console.Write("Роль (1 - Клієнт, 2 - Агент): ");
            var roleChoice = Console.ReadLine();
            user.Role = roleChoice == "2" ? UserRole.Agent : UserRole.Client;

            try
            {
                _authService.RegisterUser(user);
                Console.WriteLine("Реєстрація успішна! Тепер можете увійти.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка реєстрації: {ex.Message}");
            }
        }

        static void ShowRoleBasedMenu()
        {
            var user = _authService.GetCurrentUser();
            if (user == null) return;

            while (true)
            {
                Console.WriteLine($"\n- Головне меню ({user.Role}) -");

                switch (user.Role)
                {
                    case UserRole.Client:
                        ShowClientMenu();
                        break;
                    case UserRole.Agent:
                        ShowAgentMenu();
                        break;
                    case UserRole.Manager:
                        ShowManagerMenu();
                        break;
                }

                Console.WriteLine("0. Вийти з системи");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                if (choice == "0")
                {
                    _authService.Logout();
                    Console.WriteLine("Ви вийшли з системи.");
                    break;
                }

                HandleRoleBasedChoice(choice ?? "", user.Role);
            }
        }

        static void ShowClientMenu()
        {
            Console.WriteLine("1. Мої поліси");
            Console.WriteLine("2. Мої страхові події");
            Console.WriteLine("3. Мої платежі");
            Console.WriteLine("4. Створити запит");
            Console.WriteLine("5. Мої запити");
        }

        static void ShowAgentMenu()
        {
            Console.WriteLine("1. Клієнти");
            Console.WriteLine("2. Поліси");
            Console.WriteLine("3. Страхові події");
            Console.WriteLine("4. Запити клієнтів");
            Console.WriteLine("5. Моя статистика");
            Console.WriteLine("6. Курси валют");
        }

        static void ShowManagerMenu()
        {
            Console.WriteLine("1. Управління клієнтами");
            Console.WriteLine("2. Управління полісами");
            Console.WriteLine("3. Управління агентами");
            Console.WriteLine("4. Страхові події");
            Console.WriteLine("5. Запити клієнтів");
            Console.WriteLine("6. Платежі");
            Console.WriteLine("7. Статистика");
            Console.WriteLine("8. Звіти");
        }

        static void HandleRoleBasedChoice(string choice, UserRole role)
        {
            switch (role)
            {
                case UserRole.Client:
                    HandleClientChoice(choice);
                    break;
                case UserRole.Agent:
                    HandleAgentChoice(choice);
                    break;
                case UserRole.Manager:
                    HandleManagerChoice(choice);
                    break;
            }
        }

        static void HandleClientChoice(string choice)
        {
            var user = _authService.GetCurrentUser();
            if (user == null) return;

            switch (choice)
            {
                case "1":
                    ShowClientPolicies(user.AssociatedClientId);
                    break;
                case "2":
                    ShowClientClaims(user.AssociatedClientId);
                    break;
                case "3":
                    ShowClientPayments(user.AssociatedClientId);
                    break;
                case "4":
                    CreateClientRequestForCurrentUser();
                    break;
                case "5":
                    ShowClientRequestsForCurrentUser();
                    break;
                default:
                    Console.WriteLine("Невірний вибір!");
                    break;
            }
        }

        static void HandleAgentChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    ShowClientsMenu();
                    break;
                case "2":
                    ShowPoliciesMenu();
                    break;
                case "3":
                    ShowClaimsMenu();
                    break;
                case "4":
                    ShowClientRequestsMenu();
                    break;
                case "5":
                    ShowAgentStatistics();
                    break;
                case "6":
                    ShowExchangeRates();
                    break;
                default:
                    Console.WriteLine("Невірний вибір!");
                    break;
            }
        }

        static void HandleManagerChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    ShowClientsMenu();
                    break;
                case "2":
                    ShowPoliciesMenu();
                    break;
                case "3":
                    ShowAgentsMenu();
                    break;
                case "4":
                    ShowClaimsMenu();
                    break;
                case "5":
                    ShowClientRequestsMenu();
                    break;
                case "6":
                    ShowPaymentsMenu();
                    break;
                case "7":
                    ShowStatisticsMenu();
                    break;
                case "8":
                    ShowReportsMenu();
                    break;
                default:
                    Console.WriteLine("Невірний вибір!");
                    break;
            }
        }

        static void ShowClientPolicies(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                Console.WriteLine("Помилка: Клієнт не прив'язаний до вашого акаунта.");
                return;
            }

            var policies = _insuranceService.GetAllPolicies()
                .Where(p => p.ClientId == clientId)
                .ToList();

            Console.WriteLine("\n-МОЇ ПОЛІСИ");
            if (!policies.Any())
            {
                Console.WriteLine("Полісів не знайдено.");
                return;
            }

            foreach (var policyItem in policies)
            {
                Console.WriteLine($"Номер: {policyItem.PolicyNumber}");
                Console.WriteLine($"Тип: {policyItem.Type}");
                Console.WriteLine($"Сума покриття: {policyItem.CoverageAmount:C}");
                Console.WriteLine($"Вартість: {policyItem.Premium:C}");
                Console.WriteLine($"Статус: {policyItem.Status}");
                Console.WriteLine($"Період: {policyItem.StartDate:dd.MM.yyyy} - {policyItem.EndDate:dd.MM.yyyy}");
                Console.WriteLine("---");
            }
        }

        static void ShowClientClaims(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                Console.WriteLine("Помилка: Клієнт не прив'язаний до вашого акаунта.");
                return;
            }

            var claims = _insuranceService.GetAllClaims()
                .Where(c => c.ClientId == clientId)
                .ToList();

            Console.WriteLine("\n- Мої страхові події -");
            if (!claims.Any())
            {
                Console.WriteLine("Страхових подій не знайдено.");
                return;
            }

            foreach (var claim in claims)
            {
                Console.WriteLine($"ID: {claim.Id}");
                Console.WriteLine($"Опис: {claim.Description}");
                Console.WriteLine($"Сума: {claim.ClaimAmount:C}");
                Console.WriteLine($"Статус: {claim.Status}");
                Console.WriteLine($"Дата: {claim.ClaimDate:dd.MM.yyyy}");
                Console.WriteLine("---");
            }
        }

        static void ShowClientPayments(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                Console.WriteLine("Помилка: Клієнт не прив'язаний до вашого акаунта.");
                return;
            }

            var payments = _insuranceService.GetPayments(clientId: clientId);

            Console.WriteLine("\n- Мої платежі -");
            if (!payments.Any())
            {
                Console.WriteLine("Платежів не знайдено.");
                return;
            }

            foreach (var payment in payments)
            {
                Console.WriteLine($"Дата: {payment.PaymentDate:dd.MM.yyyy}");
                Console.WriteLine($"Сума: {payment.Amount:C}");
                Console.WriteLine($"Тип: {payment.Type}");
                Console.WriteLine($"Статус: {payment.Status}");
                Console.WriteLine($"Опис: {payment.Description}");
                Console.WriteLine("---");
            }
        }

        static void CreateClientRequestForCurrentUser()
        {
            var user = _authService.GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(user.AssociatedClientId))
            {
                Console.WriteLine("Помилка: Клієнт не прив'язаний до вашого акаунта.");
                return;
            }

            CreateClientRequestForClient(user.AssociatedClientId);
        }

        static void CreateClientRequestForClient(string clientId)
        {
            var request = new ClientRequest();
            request.ClientId = clientId;

            Console.Write("Тип страхування (1 - Авто, 2 - Медичне, 3 - Майнове): ");
            var typeChoice = Console.ReadLine();
            request.RequestedType = typeChoice switch
            {
                "1" => PolicyType.Auto,
                "2" => PolicyType.Health,
                "3" => PolicyType.Property,
                _ => PolicyType.Auto
            };

            Console.Write("Бажана сума покриття: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal coverageAmount))
            {
                Console.WriteLine("Некоректна сума!");
                return;
            }
            request.DesiredCoverageAmount = coverageAmount;

            Console.Write("Тривалість (місяців): ");
            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                Console.WriteLine("Некоректна тривалість!");
                return;
            }
            request.DurationMonths = duration;

            Console.Write("Додаткова інформація: ");
            request.AdditionalInfo = Console.ReadLine() ?? "";

            _insuranceService.AddClientRequest(request);
            Console.WriteLine("Запит успішно створено!");
        }

        static void ShowClientRequestsForCurrentUser()
        {
            var user = _authService.GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(user.AssociatedClientId))
            {
                Console.WriteLine("Помилка: Клієнт не прив'язаний до вашого акаунта.");
                return;
            }

            ShowClientRequestsForClient(user.AssociatedClientId);
        }

        static void ShowClientRequestsForClient(string clientId)
        {
            var requests = _insuranceService.GetClientRequests(clientId);
            Console.WriteLine("\n- Мої запити -");
            if (!requests.Any())
            {
                Console.WriteLine("Запитів не знайдено.");
                return;
            }

            foreach (var request in requests)
            {
                Console.WriteLine($"ID запиту: {request.Id}");
                Console.WriteLine($"Тип: {request.RequestedType}");
                Console.WriteLine($"Сума покриття: {request.DesiredCoverageAmount:C}");
                Console.WriteLine($"Тривалість: {request.DurationMonths} міс.");
                Console.WriteLine($"Статус: {request.Status}");
                Console.WriteLine($"Дата створення: {request.CreatedDate:dd.MM.yyyy}");
                Console.WriteLine("---");
            }
        }

        // Методи для агента
        static void ShowAgentStatistics()
        {
            var user = _authService.GetCurrentUser();
            if (user == null) return;

            var agentPolicies = _insuranceService.GetAllPolicies()
                .Where(p => p.AgentId == user.AssociatedAgentId)
                .ToList();

            Console.WriteLine("\n- Моя статистика -");
            Console.WriteLine($"Загальна кількість полісів: {agentPolicies.Count}");
            Console.WriteLine($"Активних полісів: {agentPolicies.Count(p => p.Status == PolicyStatus.Active)}");
            Console.WriteLine($"Загальна сума преміумів: {agentPolicies.Sum(p => p.Premium):C}");

            var agent = _insuranceService.GetAllAgents()
                .FirstOrDefault(a => a.Id == user.AssociatedAgentId);
            if (agent != null)
            {
                var totalCommission = agentPolicies.Sum(p => p.Premium * (agent.CommissionRate / 100m));
                Console.WriteLine($"Загальна комісія: {totalCommission:C}");
                Console.WriteLine($"Комісійний відсоток: {agent.CommissionRate}%");
            }
        }

        static async void ShowExchangeRates()
        {
            Console.WriteLine("\n- Курси валют -");

            var usdRate = await _apiIntegrationService.GetExchangeRate("USD", "UAH");
            var eurRate = await _apiIntegrationService.GetExchangeRate("EUR", "UAH");

            Console.WriteLine($"USD/UAH: {usdRate:F2}");
            Console.WriteLine($"EUR/UAH: {eurRate:F2}");
        }

        // Методи для менеджера
        static void ShowPaymentsMenu()
        {
            while (true)
            {
                Console.WriteLine("\n УПРАВЛІННЯ ПЛАТЕЖАМИ");
                Console.WriteLine("1. Всі платежі");
                Console.WriteLine("2. Платежі за полісом");
                Console.WriteLine("3. Статуси платежів");
                Console.WriteLine("4. Назад");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ShowAllPayments();
                        break;
                    case "2":
                        ShowPaymentsByPolicy();
                        break;
                    case "3":
                        ShowPaymentStatistics();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір!");
                        break;
                }
            }
        }

        static void ShowAllPayments()
        {
            var payments = _insuranceService.GetPayments();
            Console.WriteLine("\n- Всі платежі -");
            if (!payments.Any())
            {
                Console.WriteLine("Платежів не знайдено.");
                return;
            }

            foreach (var payment in payments)
            {
                Console.WriteLine($"ID: {payment.Id}");
                Console.WriteLine($"Тип: {payment.Type}");
                Console.WriteLine($"Сума: {payment.Amount:C}");
                Console.WriteLine($"Статус: {payment.Status}");
                Console.WriteLine($"Дата: {payment.PaymentDate:dd.MM.yyyy}");
                Console.WriteLine($"Поліс: {payment.PolicyNumber}");
                Console.WriteLine("---");
            }
        }

        static void ShowPaymentsByPolicy()
        {
            Console.Write("Введіть номер поліса: ");
            var policyNumber = Console.ReadLine();

            var payments = _insuranceService.GetPayments(policyNumber);
            Console.WriteLine($"\n- Платежі за полісом {policyNumber} -");
            if (!payments.Any())
            {
                Console.WriteLine("Платежів не знайдено.");
                return;
            }

            foreach (var payment in payments)
            {
                Console.WriteLine($"Тип: {payment.Type}");
                Console.WriteLine($"Сума: {payment.Amount:C}");
                Console.WriteLine($"Статус: {payment.Status}");
                Console.WriteLine($"Дата: {payment.PaymentDate:dd.MM.yyyy}");
                Console.WriteLine("---");
            }
        }

        static void ShowPaymentStatistics()
        {
            var financialSummary = _statisticsService.GetFinancialSummary();
            Console.WriteLine("\n- Фінансова статистика -");
            Console.WriteLine($"Загальні надходження: {financialSummary.TotalPremiums:C}");
            Console.WriteLine($"Загальні виплати: {financialSummary.TotalPayouts:C}");
            Console.WriteLine($"Баланс: {financialSummary.Balance:C}");
        }

        static void ShowReportsMenu()
        {
            Console.WriteLine("\n- Генерація звітів -");
            Console.WriteLine("1. Фінансовий звіт");
            Console.WriteLine("2. Звіт по агентам");
            Console.WriteLine("3. Звіт по типах полісів");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    GenerateFinancialReport();
                    break;
                case "2":
                    GenerateAgentsReport();
                    break;
                case "3":
                    GeneratePolicyTypeReport();
                    break;
                default:
                    Console.WriteLine("Невірний вибір!");
                    break;
            }
        }

        static void GenerateFinancialReport()
        {
            var monthlyData = _statisticsService.GetMonthlyFinancials();
            Console.WriteLine("\n- Фінансовий звіт (останні 12 місяців) -");
            foreach (var month in monthlyData)
            {
                Console.WriteLine($"{month.Month}: Надходження: {month.Premiums:C}, Виплати: {month.Payouts:C}");
            }
        }

        static void GenerateAgentsReport()
        {
            var performance = _statisticsService.GetAgentPerformance();
            Console.WriteLine("\n- Звіт по агентам -");
            foreach (var (agent, policiesSold, commission) in performance)
            {
                Console.WriteLine($"Агент: {agent.FullName}");
                Console.WriteLine($"  Продано полісів: {policiesSold}");
                Console.WriteLine($"  Загальна комісія: {commission:C}");
                Console.WriteLine("---");
            }
        }

        static void GeneratePolicyTypeReport()
        {
            var stats = _statisticsService.GetDetailedPolicyStatistics();
            Console.WriteLine("\n- Звіт по типах полісів -");
            foreach (var (type, (active, completed, expired, averagePremium)) in stats)
            {
                Console.WriteLine($"{type}:");
                Console.WriteLine($"  Активних: {active}");
                Console.WriteLine($"  Завершених: {completed}");
                Console.WriteLine($"  Закінчившихся: {expired}");
                Console.WriteLine($"  Середня вартість: {averagePremium:C}");
            }
        }

        // Загальні методи для всіх ролей
        static void ShowClientsMenu()
        {
            while (true)
            {
                Console.WriteLine("\n- Управління клієнтами -");
                Console.WriteLine("1. Додати клієнта");
                Console.WriteLine("2. Переглянути всіх клієнтів");
                Console.WriteLine("3. Пошук клієнта");
                Console.WriteLine("4. Оновити клієнта");
                Console.WriteLine("5. Видалити клієнта");
                Console.WriteLine("6. Назад");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddClient();
                        break;
                    case "2":
                        ShowAllClients();
                        break;
                    case "3":
                        SearchClient();
                        break;
                    case "4":
                        UpdateClient();
                        break;
                    case "5":
                        DeleteClient();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір!");
                        break;
                }
            }
        }

        static void AddClient()
        {
            Console.WriteLine("\n- Додавання нового клієнта -");

            var client = new Client();

            Console.Write("ПІБ: ");
            client.FullName = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            client.Email = Console.ReadLine() ?? "";

            Console.Write("Телефон: ");
            client.Phone = Console.ReadLine() ?? "";

            Console.Write("Адреса: ");
            client.Address = Console.ReadLine() ?? "";

            Console.Write("Тип (1 - Фізична особа, 2 - Юридична особа): ");
            var typeChoice = Console.ReadLine();
            client.Type = typeChoice == "2" ? ClientType.Corporate : ClientType.Individual;

            _insuranceService.AddClient(client);
            Console.WriteLine("Клієнта успішно додано!");
        }

        static void ShowAllClients()
        {
            var clients = _insuranceService.GetAllClients();
            Console.WriteLine("\n- Список клієнтів -");

            if (!clients.Any())
            {
                Console.WriteLine("Клієнтів не знайдено.");
                return;
            }

            foreach (var client in clients)
            {
                Console.WriteLine($"ID: {client.Id}");
                Console.WriteLine($"ПІБ: {client.FullName}");
                Console.WriteLine($"Email: {client.Email}");
                Console.WriteLine($"Телефон: {client.Phone}");
                Console.WriteLine($"Тип: {client.Type}");
                Console.WriteLine($"Дата реєстрації: {client.RegistrationDate:dd.MM.yyyy}");
                Console.WriteLine("---");
            }
        }

        static void SearchClient()
        {
            Console.Write("Введіть ID або ПІБ клієнта: ");
            var searchTerm = Console.ReadLine();

            var clients = _insuranceService.GetAllClients();
            var foundClients = clients.Where(c =>
                c.Id.Contains(searchTerm ?? "") ||
                c.FullName.Contains(searchTerm ?? "", StringComparison.OrdinalIgnoreCase)
            ).ToList();

            if (foundClients.Any())
            {
                Console.WriteLine("\n- Знайдені клієнти -");
                foreach (var client in foundClients)
                {
                    Console.WriteLine($"ID: {client.Id}, ПІБ: {client.FullName}, Email: {client.Email}, Телефон: {client.Phone}");
                }
            }
            else
            {
                Console.WriteLine("Клієнтів не знайдено.");
            }
        }

        static void UpdateClient()
        {
            Console.Write("Введіть ID клієнта для оновлення: ");
            var clientId = Console.ReadLine();

            var client = _insuranceService.GetClient(clientId ?? "");
            if (client == null)
            {
                Console.WriteLine("Клієнта не знайдено.");
                return;
            }

            Console.WriteLine($"Поточні дані: {client.FullName}, {client.Email}, {client.Phone}");

            Console.Write("Нове ПІБ (залиште пустим, щоб не змінювати): ");
            var newName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newName)) client.FullName = newName;

            Console.Write("Новий Email (залиште пустим, щоб не змінювати): ");
            var newEmail = Console.ReadLine();
            if (!string.IsNullOrEmpty(newEmail)) client.Email = newEmail;

            Console.Write("Новий телефон (залиште пустим, щоб не змінювати): ");
            var newPhone = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPhone)) client.Phone = newPhone;

            _insuranceService.UpdateClient(client);
            Console.WriteLine("Дані клієнта оновлено!");
        }

        static void DeleteClient()
        {
            Console.Write("Введіть ID клієнта для видалення: ");
            var clientId = Console.ReadLine();

            var client = _insuranceService.GetClient(clientId ?? "");
            if (client == null)
            {
                Console.WriteLine("Клієнта не знайдено.");
                return;
            }

            Console.Write($"Ви впевнені, що хочете видалити клієнта {client.FullName}? (y/n): ");
            var confirm = Console.ReadLine();

            if (confirm?.ToLower() == "y")
            {
                _insuranceService.DeleteClient(clientId ?? "");
                Console.WriteLine("Клієнта видалено!");
            }
            else
            {
                Console.WriteLine("Видалення скасовано.");
            }
        }

        static void ShowPoliciesMenu()
        {
            while (true)
            {
                Console.WriteLine("\n- Управління полісами -");
                Console.WriteLine("1. Додати поліс");
                Console.WriteLine("2. Переглянути всі поліси");
                Console.WriteLine("3. Пошук поліса");
                Console.WriteLine("4. Змінити статус поліса");
                Console.WriteLine("5. Назад");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddNewPolicy();
                        break;
                    case "2":
                        ShowAllPolicies();
                        break;
                    case "3":
                        SearchPolicy();
                        break;
                    case "4":
                        ShowPolicyStatusMenu();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір!");
                        break;
                }
            }
        }

        static void AddNewPolicy()
        {
            Console.WriteLine("\n- Додавання нового поліса -");

            var clients = _insuranceService.GetAllClients();
            if (!clients.Any())
            {
                Console.WriteLine("Спочатку додайте клієнтів!");
                return;
            }

            Console.WriteLine("Доступні клієнти:");
            foreach (var client in clients)
            {
                Console.WriteLine($"ID: {client.Id}, ПІБ: {client.FullName}");
            }

            Console.Write("Введіть ID клієнта: ");
            var clientId = Console.ReadLine();

            var agents = _insuranceService.GetAllAgents();
            if (agents.Any())
            {
                Console.WriteLine("Доступні агенти:");
                foreach (var agent in agents)
                {
                    Console.WriteLine($"ID: {agent.Id}, ПІБ: {agent.FullName}");
                }
                Console.Write("Введіть ID агента (залишити пустим якщо немає): ");
                var agentId = Console.ReadLine();
            }

            var newPolicy = new InsurancePolicy();
            newPolicy.ClientId = clientId ?? "";

            Console.Write("Тип поліса (1 - Авто, 2 - Медичне, 3 - Майнове): ");
            var typeChoice = Console.ReadLine();
            newPolicy.Type = typeChoice switch
            {
                "1" => PolicyType.Auto,
                "2" => PolicyType.Health,
                "3" => PolicyType.Property,
                _ => PolicyType.Auto
            };

            Console.Write("Сума покриття: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal coverageAmount))
            {
                Console.WriteLine("Некоректна сума покриття!");
                return;
            }
            newPolicy.CoverageAmount = coverageAmount;

            Console.Write("Дата початку (рррр-мм-дд): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                Console.WriteLine("Некоректна дата!");
                return;
            }
            newPolicy.StartDate = startDate;

            Console.Write("Дата завершення (рррр-мм-дд): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                Console.WriteLine("Некоректна дата!");
                return;
            }
            newPolicy.EndDate = endDate;

            _insuranceService.AddPolicy(newPolicy);
            Console.WriteLine($"Поліс успішно додано! Номер поліса: {newPolicy.PolicyNumber}");
        }

        static void ShowAllPolicies()
        {
            var policies = _insuranceService.GetAllPolicies();
            var clients = _insuranceService.GetAllClients();

            Console.WriteLine("\n- Список полісів -");

            if (!policies.Any())
            {
                Console.WriteLine("Полісів не знайдено.");
                return;
            }

            foreach (var policyItem in policies)
            {
                var client = clients.FirstOrDefault(c => c.Id == policyItem.ClientId);
                Console.WriteLine($"Номер: {policyItem.PolicyNumber}");
                Console.WriteLine($"Тип: {policyItem.Type}");
                Console.WriteLine($"Клієнт: {client?.FullName ?? "Невідомо"}");
                Console.WriteLine($"Сума покриття: {policyItem.CoverageAmount:C}");
                Console.WriteLine($"Вартість: {policyItem.Premium:C}");
                Console.WriteLine($"Статус: {policyItem.Status}");
                Console.WriteLine($"Період: {policyItem.StartDate:dd.MM.yyyy} - {policyItem.EndDate:dd.MM.yyyy}");
                Console.WriteLine("---");
            }
        }

        static void SearchPolicy()
        {
            Console.Write("Введіть номер поліса для пошуку: ");
            var policyNumber = Console.ReadLine();

            var policy = _insuranceService.GetPolicy(policyNumber ?? "");
            if (policy != null)
            {
                var clients = _insuranceService.GetAllClients();
                var client = clients.FirstOrDefault(c => c.Id == policy.ClientId);

                Console.WriteLine("\n- Знайдений поліс -");
                Console.WriteLine($"Номер: {policy.PolicyNumber}");
                Console.WriteLine($"Тип: {policy.Type}");
                Console.WriteLine($"Клієнт: {client?.FullName ?? "Невідомо"}");
                Console.WriteLine($"Сума покриття: {policy.CoverageAmount:C}");
                Console.WriteLine($"Вартість: {policy.Premium:C}");
                Console.WriteLine($"Статус: {policy.Status}");
            }
            else
            {
                Console.WriteLine("Поліс не знайдено.");
            }
        }

        static void ShowPolicyStatusMenu()
        {
            Console.Write("Введіть номер поліса: ");
            var policyNumber = Console.ReadLine();

            Console.WriteLine("Оберіть новий статус:");
            Console.WriteLine("1. Активний");
            Console.WriteLine("2. Призупинений");
            Console.WriteLine("3. Завершений");

            var choice = Console.ReadLine();
            var status = choice switch
            {
                "1" => PolicyStatus.Active,
                "2" => PolicyStatus.Suspended,
                "3" => PolicyStatus.Completed,
                _ => PolicyStatus.Active
            };

            _insuranceService.UpdatePolicyStatus(policyNumber ?? "", status);
            Console.WriteLine("Статус поліса оновлено!");
        }

        static void ShowAgentsMenu()
        {
            while (true)
            {
                Console.WriteLine("\n- Управління агентами -");
                Console.WriteLine("1. Додати агента");
                Console.WriteLine("2. Переглянути всіх агентів");
                Console.WriteLine("3. Назад");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddAgent();
                        break;
                    case "2":
                        ShowAllAgents();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір!");
                        break;
                }
            }
        }

        static void AddAgent()
        {
            Console.WriteLine("\n- Додавання нового агента -");

            var agent = new InsuranceAgent();

            Console.Write("ПІБ: ");
            agent.FullName = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            agent.Email = Console.ReadLine() ?? "";

            Console.Write("Телефон: ");
            agent.Phone = Console.ReadLine() ?? "";

            Console.Write("Відсоток комісії: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal commissionRate))
            {
                Console.WriteLine("Некоректний відсоток комісії!");
                return;
            }
            agent.CommissionRate = commissionRate;

            _insuranceService.AddAgent(agent);
            Console.WriteLine("Агента успішно додано!");
        }

        static void ShowAllAgents()
        {
            var agents = _insuranceService.GetAllAgents();
            Console.WriteLine("\n- Список агентів -");

            if (!agents.Any())
            {
                Console.WriteLine("Агентів не знайдено.");
                return;
            }

            foreach (var agent in agents)
            {
                Console.WriteLine($"ID: {agent.Id}");
                Console.WriteLine($"ПІБ: {agent.FullName}");
                Console.WriteLine($"Email: {agent.Email}");
                Console.WriteLine($"Телефон: {agent.Phone}");
                Console.WriteLine($"Комісія: {agent.CommissionRate}%");
                Console.WriteLine("---");
            }
        }

        static void ShowClaimsMenu()
        {
            while (true)
            {
                Console.WriteLine("\n- Страхові події -");
                Console.WriteLine("1. Додати страхову подію");
                Console.WriteLine("2. Переглянути всі події");
                Console.WriteLine("3. Змінити статус події");
                Console.WriteLine("4. Назад");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddClaim();
                        break;
                    case "2":
                        ShowAllClaims();
                        break;
                    case "3":
                        ShowClaimStatusMenu();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір!");
                        break;
                }
            }
        }

        static void AddClaim()
        {
            Console.WriteLine("\n- Додавання страхової події -");

            var policies = _insuranceService.GetAllPolicies();
            if (!policies.Any())
            {
                Console.WriteLine("Спочатку додайте поліси!");
                return;
            }

            Console.WriteLine("Доступні поліси:");
            foreach (var policyItem in policies)
            {
                Console.WriteLine($"Номер: {policyItem.PolicyNumber}, Тип: {policyItem.Type}");
            }

            Console.Write("Введіть номер поліса: ");
            var policyNumber = Console.ReadLine();

            var policy = _insuranceService.GetPolicy(policyNumber ?? "");
            if (policy == null)
            {
                Console.WriteLine("Поліс не знайдено!");
                return;
            }

            var claim = new InsuranceClaim();
            claim.PolicyNumber = policyNumber ?? "";
            claim.ClientId = policy.ClientId;

            Console.Write("Опис події: ");
            claim.Description = Console.ReadLine() ?? "";

            Console.Write("Сума виплати: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal claimAmount))
            {
                Console.WriteLine("Некоректна сума виплати!");
                return;
            }
            claim.ClaimAmount = claimAmount;

            _insuranceService.AddClaim(claim);
            Console.WriteLine("Страхову подію успішно додано!");
        }

        static void ShowAllClaims()
        {
            var claims = _insuranceService.GetAllClaims();
            var policies = _insuranceService.GetAllPolicies();

            Console.WriteLine("\n- Список страхових подій -");

            if (!claims.Any())
            {
                Console.WriteLine("Страхових подій не знайдено.");
                return;
            }

            foreach (var claim in claims)
            {
                var policy = policies.FirstOrDefault(p => p.PolicyNumber == claim.PolicyNumber);
                Console.WriteLine($"ID: {claim.Id}");
                Console.WriteLine($"Поліс: {claim.PolicyNumber}");
                Console.WriteLine($"Тип поліса: {policy?.Type.ToString() ?? "Невідомо"}");
                Console.WriteLine($"Дата: {claim.ClaimDate:dd.MM.yyyy}");
                Console.WriteLine($"Опис: {claim.Description}");
                Console.WriteLine($"Сума: {claim.ClaimAmount:C}");
                Console.WriteLine($"Статус: {claim.Status}");
                Console.WriteLine("---");
            }
        }

        static void ShowClaimStatusMenu()
        {
            Console.Write("Введіть ID страхової події: ");
            var claimId = Console.ReadLine();

            Console.WriteLine("Оберіть новий статус:");
            Console.WriteLine("1. Новий");
            Console.WriteLine("2. В розгляді");
            Console.WriteLine("3. Затверджено");
            Console.WriteLine("4. Виплачено");
            Console.WriteLine("5. Відхилено");

            var choice = Console.ReadLine();
            var status = choice switch
            {
                "1" => ClaimStatus.New,
                "2" => ClaimStatus.InReview,
                "3" => ClaimStatus.Approved,
                "4" => ClaimStatus.Paid,
                "5" => ClaimStatus.Rejected,
                _ => ClaimStatus.New
            };

            _insuranceService.UpdateClaimStatus(claimId ?? "", status);
            Console.WriteLine("Статус страхової події оновлено!");
        }

        static void ShowClientRequestsMenu()
        {
            while (true)
            {
                Console.WriteLine("\n- Запити клієнтів -");
                Console.WriteLine("1. Створити новий запит");
                Console.WriteLine("2. Переглянути всі запити");
                Console.WriteLine("3. Підібрати поліси під запит");
                Console.WriteLine("4. Назад");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        CreateClientRequest();
                        break;
                    case "2":
                        ShowAllClientRequests();
                        break;
                    case "3":
                        FindPoliciesForRequest();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір!");
                        break;
                }
            }
        }

        static void CreateClientRequest()
        {
            Console.WriteLine("\n- Створення нового запиту -");

            var clients = _insuranceService.GetAllClients();
            if (!clients.Any())
            {
                Console.WriteLine("Спочатку додайте клієнтів!");
                return;
            }

            Console.WriteLine("Доступні клієнти:");
            foreach (var client in clients)
            {
                Console.WriteLine($"ID: {client.Id}, ПІБ: {client.FullName}");
            }

            Console.Write("Введіть ID клієнта: ");
            var clientId = Console.ReadLine();

            var request = new ClientRequest();
            request.ClientId = clientId ?? "";

            Console.Write("Тип страхування (1 - Авто, 2 - Медичне, 3 - Майнове): ");
            var typeChoice = Console.ReadLine();
            request.RequestedType = typeChoice switch
            {
                "1" => PolicyType.Auto,
                "2" => PolicyType.Health,
                "3" => PolicyType.Property,
                _ => PolicyType.Auto
            };

            Console.Write("Бажана сума покриття: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal coverageAmount))
            {
                Console.WriteLine("Некоректна сума покриття!");
                return;
            }
            request.DesiredCoverageAmount = coverageAmount;

            Console.Write("Тривалість (місяців): ");
            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                Console.WriteLine("Некоректна тривалість!");
                return;
            }
            request.DurationMonths = duration;

            Console.Write("Додаткова інформація: ");
            request.AdditionalInfo = Console.ReadLine() ?? "";

            _insuranceService.AddClientRequest(request);
            Console.WriteLine("Запит успішно створено!");
        }

        static void ShowAllClientRequests()
        {
            var requests = _insuranceService.GetClientRequests();
            var clients = _insuranceService.GetAllClients();

            Console.WriteLine("\n- Всі запити клієнтів -");
            if (!requests.Any())
            {
                Console.WriteLine("Запитів не знайдено.");
                return;
            }

            foreach (var request in requests)
            {
                var client = clients.FirstOrDefault(c => c.Id == request.ClientId);
                Console.WriteLine($"ID запиту: {request.Id}");
                Console.WriteLine($"Клієнт: {client?.FullName ?? "Невідомо"}");
                Console.WriteLine($"Тип: {request.RequestedType}");
                Console.WriteLine($"Сума покриття: {request.DesiredCoverageAmount:C}");
                Console.WriteLine($"Тривалість: {request.DurationMonths} міс.");
                Console.WriteLine($"Статус: {request.Status}");
                Console.WriteLine($"Дата створення: {request.CreatedDate:dd.MM.yyyy}");
                Console.WriteLine("---");
            }
        }

        static void FindPoliciesForRequest()
        {
            Console.Write("Введіть ID запиту: ");
            var requestId = Console.ReadLine();

            var requests = _insuranceService.GetClientRequests();
            var request = requests.FirstOrDefault(r => r.Id == requestId);

            if (request == null)
            {
                Console.WriteLine("Запит не знайдено!");
                return;
            }

            var matchingPolicies = _insuranceService.FindMatchingPolicies(request);
            var clients = _insuranceService.GetAllClients();

            Console.WriteLine($"\n--- Поліси, що відповідають запиту {requestId} ---");
            if (!matchingPolicies.Any())
            {
                Console.WriteLine("Відповідних полісів не знайдено.");
                return;
            }

            foreach (var policyItem in matchingPolicies)
            {
                var client = clients.FirstOrDefault(c => c.Id == policyItem.ClientId);
                Console.WriteLine($"Номер: {policyItem.PolicyNumber}");
                Console.WriteLine($"Тип: {policyItem.Type}");
                Console.WriteLine($"Клієнт: {client?.FullName ?? "Невідомо"}");
                Console.WriteLine($"Сума покриття: {policyItem.CoverageAmount:C}");
                Console.WriteLine($"Вартість: {policyItem.Premium:C}");
                Console.WriteLine("---");
            }
        }

        static void ShowStatisticsMenu()
        {
            while (true)
            {
                Console.WriteLine("\n- Статистика та аналітика -");
                Console.WriteLine("1. Загальна статистика");
                Console.WriteLine("2. Статистика по типах полісів");
                Console.WriteLine("3. Ефективність агентів");
                Console.WriteLine("4. Статистика страхових подій");
                Console.WriteLine("5. Назад");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ShowGeneralStatistics();
                        break;
                    case "2":
                        ShowPolicyTypeStatistics();
                        break;
                    case "3":
                        ShowAgentPerformance();
                        break;
                    case "4":
                        ShowClaimsStatistics();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір!");
                        break;
                }
            }
        }

        static void ShowGeneralStatistics()
        {
            Console.WriteLine("\n- Загальна статистика компанії -");

            var activePolicies = _statisticsService.GetActivePoliciesCount();
            var totalClaims = _statisticsService.GetClaimsCount();
            var totalPayouts = _statisticsService.GetTotalPayouts();
            var revenue = _statisticsService.GetCompanyRevenue();

            Console.WriteLine($"Активних полісів: {activePolicies}");
            Console.WriteLine($"Всього страхових подій: {totalClaims}");
            Console.WriteLine($"Загальна сума виплат: {totalPayouts:C}");
            Console.WriteLine($"Дохід компанії: {revenue:C}");
        }

        static void ShowPolicyTypeStatistics()
        {
            Console.WriteLine("\n- Статистика по типах полісів -");

            var stats = _statisticsService.GetPolicyTypeStatistics();
            foreach (var (type, (count, totalPremium)) in stats)
            {
                Console.WriteLine($"{type}:");
                Console.WriteLine($"  Кількість: {count}");
                Console.WriteLine($"  Загальна вартість: {totalPremium:C}");
                if (count > 0)
                    Console.WriteLine($"  Середня вартість: {totalPremium / count:C}");
            }
        }

        static void ShowAgentPerformance()
        {
            Console.WriteLine("\n- Ефективність агентів -");

            var performance = _statisticsService.GetAgentPerformance();
            foreach (var (agent, policiesSold, commission) in performance)
            {
                Console.WriteLine($"Агент: {agent.FullName}");
                Console.WriteLine($"  Продано полісів: {policiesSold}");
                Console.WriteLine($"  Загальна комісія: {commission:C}");
                Console.WriteLine($"  Комісійний відсоток: {agent.CommissionRate}%");
                Console.WriteLine("---");
            }
        }

        static void ShowClaimsStatistics()
        {
            Console.WriteLine("\n- Статистика страхових подій -");

            var claimsByStatus = _statisticsService.GetClaimsByStatus();
            foreach (var (status, count) in claimsByStatus)
            {
                Console.WriteLine($"{status}: {count}");
            }
        }
    }
}