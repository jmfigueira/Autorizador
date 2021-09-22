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
    public class TransactionTests
    {
        private IAccountService _accountService;
        private ITransactionService _transactionService;
        private ILogger _logger;
        private ITransactionRepository _transactionRepository;
        private IAccountRepository _accountRepository;
        private PersistContext _context;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger>();

            var transactionRepository = new Mock<ITransactionRepository>();
            transactionRepository.Setup(m => m.Insert(It.IsAny<Transaction>()));

            _logger = mockLogger.Object;
            _context = new PersistContext();

            _transactionRepository = new TransactionRepository(_context);
            _accountRepository = new AccountRepository(_context);

            _transactionService = new TransactionService(_transactionRepository, _accountRepository);
            _accountService = new AccountService(_accountRepository);
        }

        [TestCase("{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 25, \"time\": \"2020-12-01T11: 07:00.000Z\"}}")]
        public void NaoDeveInserirUmaTransacaoSemContaCriada(string transaction)
        {
            var returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction).Transaction);

            Assert.IsTrue(returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)));
        }

        [TestCase("{\"account\": {\"active-card\": true, \"available-limit\": 100}}", "{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 25, \"time\": \"2020-12-01T11: 07:00.000Z\"}}")]
        public void DeveInserirUmaTransacao(string account, string transaction)
        {
            var returns = _accountService.Insert(JsonConvert.DeserializeObject<AccountData>(account).Account);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) || !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));
        }

        [TestCase("{\"account\": {\"active-card\": false, \"available-limit\": 100}}", "{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 25, \"time\": \"2020-12-01T11: 07:00.000Z\"}}")]
        public void NaoDeveInserirUmaTransacaoQuandoCartaoNaoAtivo(string account, string transaction)
        {
            var returns = _accountService.Insert(JsonConvert.DeserializeObject<AccountData>(account).Account);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction).Transaction);

            Assert.IsTrue(returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));
        }

        [TestCase("{\"account\": {\"active-card\": true, \"available-limit\": 100}}", "{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 250, \"time\": \"2020-12-01T11: 07:00.000Z\"}}")]
        public void NaoDeveInserirUmaTransacaoPorFaltaDeLimite(string account, string transaction)
        {
            var returns = _accountService.Insert(JsonConvert.DeserializeObject<AccountData>(account).Account);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction).Transaction);

            Assert.IsTrue(returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)));
        }

        [TestCase("{\"account\": {\"active-card\": true, \"available-limit\": 100}}",
                  "{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 25, \"time\": \"2020-12-01T11: 07:00.000Z\"}}",
                  "{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 25, \"time\": \"2020-12-01T11: 07:02.000Z\"}}")]
        public void NaoDevePermitirInserirDuasTransacoesNumIntervalorDeDoisMinutosParaMesmoDestinoEValor(string account, string transaction1, string transaction2)
        {
            var returns = _accountService.Insert(JsonConvert.DeserializeObject<AccountData>(account).Account);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction1).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction2).Transaction);

            Assert.IsTrue(returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)));
        }

        [TestCase("{\"account\": {\"active-card\": true, \"available-limit\": 100}}",
                  "{\"transaction\": {\"merchant\": \"Ifood\", \"amount\": 25, \"time\": \"2020-12-01T11: 07:00.000Z\"}}",
                  "{\"transaction\": {\"merchant\": \"Uber Eats\", \"amount\": 25, \"time\": \"2020-12-01T11: 07:01.000Z\"}}",
                  "{\"transaction\": {\"merchant\": \"Netflix\", \"amount\": 25, \"time\": \"2020-12-01T11: 07:02.000Z\"}}",
                  "{\"transaction\": {\"merchant\": \"Subway\", \"amount\": 25, \"time\": \"2020-12-01T11: 07:03.000Z\"}}")]
        public void DevePermitirInserirMaisNoMaximoTresTransacoesNumIntervalorDeDoisMinutosParaDiferentesDestinosEValores(string account, string transaction1, string transaction2, string transaction3, string transaction4)
        {
            var returns = _accountService.Insert(JsonConvert.DeserializeObject<AccountData>(account).Account);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction1).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction2).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction3).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction4).Transaction);

            Assert.IsTrue(returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)));
        }

        [TestCase("{\"account\": {\"active-card\": true, \"available-limit\": 100}}",
                  "{\"transaction\": {\"merchant\": \"McDonald's\", \"amount\": 10, \"time\": \"2019-02-13T11:00:01.000Z\"}}",
                  "{\"transaction\": {\"merchant\": \"Burger King\", \"amount\": 20,\"time\": \"2019-02-13T11:00:02.000Z\"}}",
                  "{\"transaction\": {\"merchant\": \"Burger King\", \"amount\": 5, \"time\": \"2019-02-13T11:00:07.000Z\"}}",
                  "{\"transaction\": {\"merchant\": \"Burger King\", \"amount\": 5, \"time\": \"2019-02-13T11:00:08.000Z\"}}")]
        public void DeveViolarAltaFrequenciaEDuplaTransacao(string account, string transaction1, string transaction2, string transaction3, string transaction4)
        {
            var returns = _accountService.Insert(JsonConvert.DeserializeObject<AccountData>(account).Account);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction1).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction2).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction3).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction4).Transaction);

            Assert.IsTrue(returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) && returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)));
        }

        [TestCase("{\"account\": {\"active-card\": true, \"available-limit\": 100}}",
                  "{\"transaction\": {\"merchant\": \"McDonald's\", \"amount\": 10, \"time\": \"2019-02-13T11:00:01.000Z\"}}",
                  "{\"transaction\": {\"merchant\": \"Burger King\", \"amount\": 20,\"time\": \"2019-02-13T11:00:02.000Z\"}}",
                  "{\"transaction\": {\"merchant\": \"Burger King\", \"amount\": 5, \"time\": \"2019-02-13T11:00:07.000Z\"}}",
                  "{\"transaction\": {\"merchant\": \"Burger King\", \"amount\": 150, \"time\": \"2019-02-13T11: 00:18.000Z\"}}")]
        public void DeveViolarAltaFrequenciaELimiteTransacao(string account, string transaction1, string transaction2, string transaction3, string transaction4)
        {
            var returns = _accountService.Insert(JsonConvert.DeserializeObject<AccountData>(account).Account);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountAlreadyInitialized)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction1).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction2).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction3).Transaction);

            Assert.IsTrue(!returns.Contains(Enumerations.GetEnumDescription(Violations.AccountNotInitialized)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.DoubledTransaction)) ||
                          !returns.Contains(Enumerations.GetEnumDescription(Violations.CardNotActive)));

            returns = _transactionService.Insert(JsonConvert.DeserializeObject<TransactionData>(transaction4).Transaction);

            Assert.IsTrue(returns.Contains(Enumerations.GetEnumDescription(Violations.HighFrequencySmallInterval)) && returns.Contains(Enumerations.GetEnumDescription(Violations.InsufficientLimit)));
        }
    }
}