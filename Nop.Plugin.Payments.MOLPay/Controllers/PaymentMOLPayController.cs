using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.MOLPay.Models;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Security.Cryptography;

namespace Nop.Plugin.Payments.MOLPay.Controllers
{
    public class PaymentMOLPayController : BasePaymentController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly PaymentSettings _paymentSettings;
        private readonly MOLPayPaymentSettings _molPayPaymentSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentMOLPayController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IPermissionService permissionService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger,
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            MOLPayPaymentSettings molPayPaymentSettings,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._permissionService = permissionService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._logger = logger;
            this._webHelper = webHelper;
            this._paymentSettings = paymentSettings;
            this._molPayPaymentSettings = molPayPaymentSettings;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods  

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var molPayPaymentSettings = _settingService.LoadSetting<MOLPayPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                UseSandbox = molPayPaymentSettings.UseSandbox,
                MerchantID = molPayPaymentSettings.MerchantID,
                VerifyKey = molPayPaymentSettings.VerifyKey,
                SecretKey = molPayPaymentSettings.SecretKey,
                CapturedModeId = (int)molPayPaymentSettings.CapturedMode,
                CapturedModes = molPayPaymentSettings.CapturedMode.ToSelectList(),
                FailedModeId = (int)molPayPaymentSettings.FailedMode,
                FailedModes = molPayPaymentSettings.FailedMode.ToSelectList(),
                PendingModeId = (int)molPayPaymentSettings.PendingMode,
                PendingModes = molPayPaymentSettings.PendingMode.ToSelectList(),

                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope > 0)
            {
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(molPayPaymentSettings, x => x.UseSandbox, storeScope);
                model.MerchantID_OverrideForStore = _settingService.SettingExists(molPayPaymentSettings, x => x.MerchantID, storeScope);
                model.VerifyKey_OverrideForStore = _settingService.SettingExists(molPayPaymentSettings, x => x.VerifyKey, storeScope);
                model.SecretKey_OverrideForStore = _settingService.SettingExists(molPayPaymentSettings, x => x.SecretKey, storeScope);
                model.CapturedModeId_OverrideForStore = _settingService.SettingExists(molPayPaymentSettings, x => x.CapturedModeId, storeScope);
                model.CapturedModes_OverrideForStore = _settingService.SettingExists(molPayPaymentSettings, x => x.CapturedModes, storeScope);
                model.FailedModeId_OverrideForStore = _settingService.SettingExists(molPayPaymentSettings, x => x.FailedModeId, storeScope);
                model.FailedModes_OverrideForStore = _settingService.SettingExists(molPayPaymentSettings, x => x.FailedModes, storeScope);
                model.PendingModeId_OverrideForStore = _settingService.SettingExists(molPayPaymentSettings, x => x.PendingModeId, storeScope);
                model.PendingModes_OverrideForStore = _settingService.SettingExists(molPayPaymentSettings, x => x.PendingModes, storeScope);
            }

            return View("~/Plugins/Payments.MOLPay/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var molPayPaymentSettings = _settingService.LoadSetting<MOLPayPaymentSettings>(storeScope);

            //save settings
            molPayPaymentSettings.UseSandbox = model.UseSandbox;
            molPayPaymentSettings.MerchantID = model.MerchantID;
            molPayPaymentSettings.VerifyKey = model.VerifyKey;
            molPayPaymentSettings.SecretKey = model.SecretKey;
            molPayPaymentSettings.CapturedModeId = model.CapturedModeId;
            molPayPaymentSettings.CapturedMode = MOLPayHelper.GetPaymentTitle(model.CapturedModeId);
            molPayPaymentSettings.FailedModeId = model.FailedModeId;
            molPayPaymentSettings.FailedMode = MOLPayHelper.GetPaymentTitle(model.FailedModeId);
            molPayPaymentSettings.PendingModeId = model.PendingModeId;
            molPayPaymentSettings.PendingMode = MOLPayHelper.GetPaymentTitle(model.PendingModeId);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(molPayPaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(molPayPaymentSettings, x => x.MerchantID, model.MerchantID_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(molPayPaymentSettings, x => x.VerifyKey, model.VerifyKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(molPayPaymentSettings, x => x.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(molPayPaymentSettings, x => x.CapturedModeId, model.CapturedModeId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(molPayPaymentSettings, x => x.CapturedMode, model.CapturedModes_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(molPayPaymentSettings, x => x.FailedModeId, model.FailedModeId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(molPayPaymentSettings, x => x.FailedMode, model.FailedModes_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(molPayPaymentSettings, x => x.PendingModeId, model.PendingModeId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(molPayPaymentSettings, x => x.PendingMode, model.PendingModes_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        //action displaying notification (warning) to a store owner about inaccurate MOLPay rounding
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult RoundingWarning(bool passProductNamesAndTotals)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prices and total aren't rounded, so display warning
            if (passProductNamesAndTotals && !_shoppingCartSettings.RoundPricesDuringCalculation)
                return Json(new { Result = _localizationService.GetResource("Plugins.Payments.MOLPay.RoundingWarning") });

            return Json(new { Result = string.Empty });
        }

        private string md5encode(string input)
        {
            using (MD5 hasher = MD5.Create())
            {

                byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder str = new StringBuilder();
                for (int n = 0; n <= hash.Length - 1; n++)
                {
                    str.Append(hash[n].ToString("X2"));
                }

                return str.ToString().ToLower();
            }
        }

        public IActionResult PDTHandler(IFormCollection form)
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.MOLPay") as MOLPayPaymentProcessor;

            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
            {
                throw new NopException("MOLPay module cannot be loaded");
            }

            var skey = form["skey"];
            var tranID = form["tranID"];
            var domain = form["domain"];
            var status = form["status"];
            var amount = form["amount"];
            var currency = form["currency"];
            var paydate = form["paydate"];
            int orderid = Int32.Parse(form["orderid"]);
            var appcode = form["appcode"];
            var error_code = form["error_code"];
            var error_desc = form["error_desc"];
            var channel = form["channel"];
            var vkey = _molPayPaymentSettings.VerifyKey;
            var mc_gross = decimal.Zero;
            try
            {
                mc_gross = decimal.Parse(form["mc_gross"], new CultureInfo("en-US"));
            }
            catch (Exception exc)
            {
                _logger.Error("MOLPay PDT. Error getting mc_gross", exc);
            }

            if (tranID != "")
            {
                var order = _orderService.GetOrderById(orderid);
                //if (order != null)
                //{

                    var sb = new StringBuilder();
                    sb.AppendLine("MOLPay PDT:");
                    sb.AppendLine("tranID: " + tranID);
                    sb.AppendLine("amount: " + amount);
                    sb.AppendLine("currency: " + currency);
                    sb.AppendLine("Pending reason: " + string.Empty);
                    sb.AppendLine("paydate: " + paydate);
                    sb.AppendLine("orderid: " + orderid);
                    sb.AppendLine("channel: " + channel);

                    var captured = _molPayPaymentSettings.CapturedMode;
                    var failed = _molPayPaymentSettings.FailedMode;
                    var pending = _molPayPaymentSettings.PendingMode;
                    var result = pending;
                    switch (status)
                    {
                        case "00":
                            result = captured;
                            break;
                        case "11":
                            result = failed;
                            break;
                        case "22":
                            result = pending;
                            break;
                        default:
                            break;
                    }

                //var newPaymentStatus = MOLPayHelper.GetPaymentStatus(status);
                    var newPaymentStatus = result;
                    sb.AppendLine("New payment status: " + newPaymentStatus);

                    //order note
                    order.OrderNotes.Add(new OrderNote
                    {
                        Note = sb.ToString(),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(order);

                //validate order total
                var orderTotalSentToMOLPay = order.GetAttribute<decimal?>(MOLPayHelper.OrderTotalSentToMOLPay);
                if (orderTotalSentToMOLPay.HasValue && mc_gross != orderTotalSentToMOLPay.Value)
                {
                    var errorStr =
                        $"MOLPay PDT. Returned order total {mc_gross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
                    //log
                    _logger.Error(errorStr);
                    //order note
                    order.OrderNotes.Add(new OrderNote
                    {
                        Note = errorStr,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(order);

                    return RedirectToAction("Index", "Home", new { area = "" });
                }

                //clear attribute
                if (orderTotalSentToMOLPay.HasValue)
                    _genericAttributeService.SaveAttribute<decimal?>(order, MOLPayHelper.OrderTotalSentToMOLPay, null);

                //mark order as paid
                if (newPaymentStatus == captured)
                {
                    if (_orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.AuthorizationTransactionId = tranID;
                        _orderService.UpdateOrder(order);

                        _orderProcessingService.MarkOrderAsPaid(order);
                    }
                }

                order.PaymentStatus = result;

                    //if(order.PaymentStatus == PaymentStatus.Paid)
                    //{
                    //    order.PaymentStatus = _molPayPaymentSettings.CapturedMode;
                    //}
                    //else if (order.PaymentStatus == PaymentStatus.Pending)
                    //{
                    //    order.PaymentStatus = _molPayPaymentSettings.PendingMode;
                    //}
                    //else
                    //{
                    //    order.PaymentStatus = _molPayPaymentSettings.FailedMode;
                    //}

                    var key0 = md5encode(tranID + orderid + status + domain + amount + currency);
                    var key1 = md5encode(paydate + domain + key0 + appcode + vkey);
                    
                    if (skey != key1)
                    {
                        order.OrderNotes.Add(new OrderNote
                        {
                            Note = "Invalid Transaction.",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _orderService.UpdateOrder(order);

                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                    else
                    {
                        switch (status)
                        {
                            case "00":
                                order.OrderNotes.Add(new OrderNote
                                {
                                    Note = "Captured.",
                                    DisplayToCustomer = false,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                                _orderService.UpdateOrder(order);
                                return RedirectToRoute("CheckoutCompleted", new { orderId = orderid });
                            case "11":
                                order.OrderNotes.Add(new OrderNote
                                {
                                    Note = "Failed.",
                                    DisplayToCustomer = false,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                                _orderService.UpdateOrder(order);
                                return RedirectToAction("Index", "Home", new { area = "" });
                            case "22":
                                order.OrderNotes.Add(new OrderNote
                                {
                                    Note = "Pending.",
                                    DisplayToCustomer = false,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                                _orderService.UpdateOrder(order);
                                return RedirectToAction("Index", "Home", new { area = "" });
                        }
                    }
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public IActionResult IPNHandler(IFormCollection form)
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.MOLPay") as MOLPayPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("MOLPay module cannot be loaded");

                #region values
            var mc_gross = decimal.Zero;
            try
            {
                mc_gross = decimal.Parse(form["mc_gross"], new CultureInfo("en-US"));
            }
            catch { }

            #endregion

            var skey = form["skey"];
            var tranID = form["tranID"];
            var domain = form["domain"];
            var status = form["status"];
            var amount = form["amount"];
            var currency = form["currency"];
            var paydate = form["paydate"];
            var orderid = form["orderid"];
            var appcode = form["appcode"];
            var channel = form["channel"];
            var nbcb = form["nbcb"];
            var vkey = _molPayPaymentSettings.VerifyKey;

            var key0 = md5encode(tranID + orderid + status + domain + amount + currency);
            var key1 = md5encode(paydate + domain + key0 + appcode + vkey);

            var order = _orderService.GetOrderById(Int32.Parse(orderid));

            if (skey == key1)
            {
                if(nbcb == "1" || nbcb == "2")
                { 
                    var sb = new StringBuilder();
                    sb.AppendLine("MOLPay IPN:");
                    foreach (var kvp in form)
                    {
                        sb.AppendLine(kvp.Key + ": " + kvp.Value);
                    }

                    var captured = _molPayPaymentSettings.CapturedMode;
                    var failed = _molPayPaymentSettings.FailedMode;
                    var pending = _molPayPaymentSettings.PendingMode;
                    var result = pending;
                    switch (status)
                    {
                        case "00":
                            result = captured;
                            break;
                        case "11":
                            result = failed;
                            break;
                        case "22":
                            result = pending;
                            break;
                        default:
                            break;
                    }

                    var newPaymentStatus = result;
                    sb.AppendLine("New payment status: " + newPaymentStatus);

                    //order note
                    order.OrderNotes.Add(new OrderNote
                    {
                        Note = sb.ToString(),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(order);

                    //validate order total
                    var orderTotalSentToMOLPay = order.GetAttribute<decimal?>(MOLPayHelper.OrderTotalSentToMOLPay);
                    if (orderTotalSentToMOLPay.HasValue && mc_gross != orderTotalSentToMOLPay.Value)
                    {
                        var errorStr =
                            $"MOLPay PDT. Returned order total {mc_gross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
                        //log
                        _logger.Error(errorStr);
                        //order note
                        order.OrderNotes.Add(new OrderNote
                        {
                            Note = errorStr,
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _orderService.UpdateOrder(order);

                        return RedirectToAction("Index", "Home", new { area = "" });
                    }

                    //clear attribute
                    if (orderTotalSentToMOLPay.HasValue)
                        _genericAttributeService.SaveAttribute<decimal?>(order, MOLPayHelper.OrderTotalSentToMOLPay, null);

                    //mark order as paid
                    if (newPaymentStatus == captured)
                    {
                        if (_orderProcessingService.CanMarkOrderAsPaid(order))
                        {
                            order.AuthorizationTransactionId = tranID;
                            _orderService.UpdateOrder(order);

                            _orderProcessingService.MarkOrderAsPaid(order);
                        }
                    }

                    order.PaymentStatus = result;
                    _orderService.UpdateOrder(order);

                }
            }

            if (nbcb == "1")
            {
                return Content(Response.WriteAsync("CBTOKEN:MPSTATOK").ToString());
            }

            return Content("");
        }

        public IActionResult CancelOrder()
        {
            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();
            if (order != null)
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });

            return RedirectToRoute("HomePage");
        }

        #endregion
    }
}