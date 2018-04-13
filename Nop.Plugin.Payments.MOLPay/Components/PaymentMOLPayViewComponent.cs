using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Payments.MOLPay.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.MOLPay.Components
{
    [ViewComponent(Name = "PaymentMOLPay")]
    public class PaymentMOLPayViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel()
            {
                ChannelTypes = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Visa/Master card", Value = "credit" },
                    new SelectListItem { Text = "Maybank2u", Value = "maybank2u" },
                    new SelectListItem { Text = "CIMB Clicks", Value = "cimbclicks" },
                    new SelectListItem { Text = "7e", Value = "7e" },
                }
            };

            return View("~/Plugins/Payments.MOLPay/Views/PaymentInfo.cshtml",model);
        }
    }
}
