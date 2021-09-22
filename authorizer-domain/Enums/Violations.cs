using System.ComponentModel;

namespace authorizer_domain.Enums
{
    public enum Violations
    {
        [Description("insufficient-limit")]
        InsufficientLimit = 0,
        [Description("account-already-initialized")]
        AccountAlreadyInitialized = 1,
        [Description("card-not-active")]
        CardNotActive = 2,
        [Description("high-frequency-small-interval")]
        HighFrequencySmallInterval = 3,
        [Description("doubled-transaction")]
        DoubledTransaction = 4,
        [Description("account-not-initialized")]
        AccountNotInitialized = 5
    }
}