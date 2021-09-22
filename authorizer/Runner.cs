using authorizer_domain.Entities;
using authorizer_domain.Interfaces;
using authorizer_infra_shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace authorizer
{
    public class Runner : IRunner
    {
        private readonly ICommandValidator _commandValidator;
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;
        private readonly ILogger<Runner> _logger;

        public Runner(IAccountService accountService, ITransactionService transactionService, ILoggerFactory loggerFactory, ICommandValidator commandValidator)
        {
            _accountService = accountService;
            _transactionService = transactionService;
            _logger = loggerFactory.CreateLogger<Runner>();
            _commandValidator = commandValidator;
        }

        public void Run()
        {
            Console.WriteLine($"To close the program, type: close\n");

            Console.WriteLine(">>");

            var command = string.Empty;
            do
            {
                command = Console.ReadLine();

                if (!_commandValidator.IsValid(command))
                    Console.WriteLine($"Sorry, input: '{command}' not recognized.\n\n>>");
                else
                {
                    try
                    {
                        bool isValidObject = command.TryParseJson<AccountData>(out AccountData resultAccountData);

                        if (isValidObject)
                        {
                            var accountData = JsonConvert.DeserializeObject<AccountData>(command);

                            var std = _accountService.Insert(accountData.Account);

                            Console.WriteLine($"\nOUTPUT >> {std}\n\n>>");
                        }
                        else
                        {
                            isValidObject = command.TryParseJson<TransactionData>(out TransactionData resultTransactionData);

                            if (isValidObject)
                            {
                                var transactionData = JsonConvert.DeserializeObject<TransactionData>(command);

                                var std = _transactionService.Insert(transactionData.Transaction);

                                Console.WriteLine($"\nOUTPUT >> {std}\n\n>>");
                            }
                            else
                                throw new Exception();
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Sorry, input: '{command}' not recognized.\n\n>>");
                    }
                }
            }
            while (!command.Equals("close", StringComparison.InvariantCultureIgnoreCase));

            Console.ReadKey();
        }
    }
}