using FluentValidation;
using Nop.Web.Framework.Validators;
using Nop.Plugin.Payments.MOLPay.Models;

namespace Nop.Plugin.Payments.MOLPay.Validators
{
    public partial class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationModelValidator()
        {
            RuleFor(model => model.MerchantID).Must((model, context) =>
            {
            return !string.IsNullOrEmpty(model.MerchantID);
            }).WithMessage($"Merchant ID must no be empty");

            RuleFor(model => model.VerifyKey).Must((model, context) =>
            {
                return !string.IsNullOrEmpty(model.VerifyKey);
            }).WithMessage($"Verify key must no be empty");

            RuleFor(model => model.SecretKey).Must((model, context) =>
            {
                return !string.IsNullOrEmpty(model.SecretKey);
            }).WithMessage($"Secret key must no be empty");
        }
    }
}
