using System;
using Nop.Core.Configuration;
using Nop.Core.Domain.Payments;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Payments.MOLPay
{
    /// <summary>
    /// Represents settings of the MOLPay payment plugin
    /// </summary>
    public class MOLPayPaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a business email
        /// </summary>
        public string BusinessEmail { get; set; }

        /// <summary>
        /// Gets or sets PDT identity token
        /// </summary>
        public string PdtToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to pass info about purchased items to MOLPay
        /// </summary>
        public bool PassProductNamesAndTotals { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        public string MerchantID { get; set; }

        public int CapturedModeId { get; set; }

        public SelectList CapturedModes { get; set; }

        public int FailedModeId { get; set; }

        public SelectList FailedModes { get; set; }

        public int PendingModeId { get; set; }

        public SelectList PendingModes { get; set; }

        public string VerifyKey { get; set; }

        public string SecretKey { get; set; }

        public string ChannelType { get; set; }

        public PaymentStatus CapturedMode { get; set; }

        public PaymentStatus FailedMode { get; set; }

        public PaymentStatus PendingMode { get; set; }

    }
}
