using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Payments.MOLPay.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public PaymentInfoModel()
        {
            ChannelTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.Channel")]
        public string ChannelType { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MOLPay.Fields.Channel")]
        public IList<SelectListItem> ChannelTypes { get; set; }
    }
}
