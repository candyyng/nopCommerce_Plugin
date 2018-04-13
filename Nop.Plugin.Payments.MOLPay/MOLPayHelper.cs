using Nop.Core.Domain.Payments;

namespace Nop.Plugin.Payments.MOLPay
{
    public class MOLPayHelper
    {
        #region Properties

        /// <summary>
        /// Get nopCommerce partner code
        /// </summary>
        public static string NopCommercePartnerCode => "nopCommerce_SP";

        /// <summary>
        /// Get the generic attribute name that is used to store an order total that actually sent to MOLPay (used to PDT order total validation)
        /// </summary>
        public static string OrderTotalSentToMOLPay => "OrderTotalSentToMOLPay";

        #endregion

        #region Methods

        public static PaymentStatus GetPaymentTitle(int payID)
        {
            var result = PaymentStatus.Pending;

            switch(payID)
            {
                case 10:
                    result = PaymentStatus.Pending;
                    break;
                case 20:
                    result = PaymentStatus.Authorized;
                    break;
                case 30:
                    result = PaymentStatus.Paid;
                    break;
                case 35:
                    result = PaymentStatus.PartiallyRefunded;
                    break;
                case 40:
                    result = PaymentStatus.Refunded;
                    break;
                case 50:
                    result = PaymentStatus.Voided;
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets a payment status
        /// </summary>
        /// <param name="paymentStatus">MOLPay payment status</param>
        /// <param name="pendingReason">MOLPay pending reason</param>
        /// <returns>Payment status</returns>
        public static PaymentStatus GetPaymentStatus(string paymentStatus)
        {
            var result = PaymentStatus.Pending;

            if (paymentStatus == null)
                paymentStatus = string.Empty;

            switch (paymentStatus)
            {
                //case "pending":
                //    switch (pendingReason.ToLowerInvariant())
                //    {
                //        case "authorization":
                //            result = PaymentStatus.Authorized;
                //            break;
                //        default:
                //            result = PaymentStatus.Pending;
                //            break;
                //    }
                //    break;
                case "00":
                    result = PaymentStatus.Paid;
                    break;
                case "11":
                    result = PaymentStatus.Voided;
                    break;
                case "22":
                    result = PaymentStatus.Pending;
                    break;
                case "denied":
                case "expired":
                case "failed":
                case "voided":
                    result = PaymentStatus.Voided;
                    break;
                case "refunded":
                case "reversed":
                    result = PaymentStatus.Refunded;
                    break;
                default:
                    break;
            }
            return result;
        }

        #endregion
    }
}
