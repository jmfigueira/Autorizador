using authorizer_domain.Entities;
using authorizer_domain.Enums;
using authorizer_domain.Interfaces;
using authorizer_infra.Context;
using authorizer_infra.Repository;
using authorizer_infra_shared;
using authorizer_service.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace authorizer_tests
{
    public class AccountTests
    {
        private IAccountService _accountService;
        private ILogger _logger;
        private IAccountRepository _accountRepository;
        private PersistContext _context;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger>();

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(m => m.Insert(It.IsAny<Account>()));

            _logger = mockLogger.Object;
            _context = new PersistContext();

            _accountRepository = new AccountRepository(_context);

            _accountService = new AccountService(_accountRepository);
        }

        [TestCase("{\"account\": {\"active-card\": true, \"available-limit\": 100}}")]
        public void DeveInserirUmaNovaConta(string account)
        {
            var returns = _accountService.Insert(JsonConvert.DeserializeObject<AccountData>(account).Account);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized)));
        }

        [TestCase("{\"account\": {\"active-card\": true, \"available-limit\": 100}}", "{\"account\": {\"active-card\": true, \"available-limit\": 150}}")]
        public void NaoDevePermitirInserirDuasOuMaisContas(string account1, string account2)
        {
            var returns = _accountService.Insert(JsonConvert.DeserializeObject<AccountData>(account1).Account);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized)));

            returns = _accountService.Insert(JsonConvert.DeserializeObject<AccountData>(account2).Account);

            Assert.IsTrue(returns.Contains(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized)));
        }
    }
}