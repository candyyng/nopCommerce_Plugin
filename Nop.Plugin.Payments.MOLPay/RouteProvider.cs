using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.MOLPay
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //PDT
            routeBuilder.MapRoute("Plugin.Payments.MOLPay.PDTHandler", "Plugins/PaymentMOLPay/PDTHandler",
                 new { controller = "PaymentMOLPay", action = "PDTHandler" });

            //IPN
            routeBuilder.MapRoute("Plugin.Payments.MOLPay.IPNHandler", "Plugins/PaymentMOLPay/IPNHandler",
                 new { controller = "PaymentMOLPay", action = "IPNHandler" });

            //Cancel
            routeBuilder.MapRoute("Plugin.Payments.MOLPay.CancelOrder", "Plugins/PaymentMOLPay/CancelOrder",
                 new { controller = "PaymentMOLPay", action = "CancelOrder" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            get { return -1; }
        }
    }
}
