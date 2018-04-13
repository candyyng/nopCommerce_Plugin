using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using Nop.Plugin.Payments.MOLPay.Validators;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Plugin.Payments.MOLPay.Models
{
    [Validator(typeof(ConfigurationModelValidator))]
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.BusinessEmail")]
        //public string BusinessEmail { get; set; }
        //public bool BusinessEmail_OverrideForStore { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.PDTToken")]
        //public string PdtToken { get; set; }
        //public bool PdtToken_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }
        public bool PassProductNamesAndTotals_OverrideForStore { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.AdditionalFee")]
        //public decimal AdditionalFee { get; set; }
        //public bool AdditionalFee_OverrideForStore { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.AdditionalFeePercentage")]
        //public bool AdditionalFeePercentage { get; set; }
        //public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.MerchantID")]
        public string MerchantID { get; set; }
        public bool MerchantID_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.VerifyKey")]
        public string VerifyKey { get; set; }
        public bool VerifyKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.SecretKey")]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.CapturedMode")]
        public int CapturedModeId { get; set; }
        public bool CapturedModeId_OverrideForStore { get; set; }
        public SelectList CapturedModes { get; set; }
        public bool CapturedModes_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.FailedMode")]
        public int FailedModeId { get; set; }
        public bool FailedModeId_OverrideForStore { get; set; }
        public SelectList FailedModes { get; set; }
        public bool FailedModes_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.PendingMode")]
        public int PendingModeId { get; set; }
        public bool PendingModeId_OverrideForStore { get; set; }
        public SelectList PendingModes { get; set; }
        public bool PendingModes_OverrideForStore { get; set; }
    }
}
