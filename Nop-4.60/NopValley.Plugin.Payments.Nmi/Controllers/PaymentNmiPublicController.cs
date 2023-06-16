using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Microsoft.AspNetCore.Http;
using Nop.Services.Payments;
using Nop.Services.Common;
using Nop.Core.Domain.Customers;
using NopValley.Plugin.Payments.Nmi.Models;
using System.Net;
using System.Collections.Specialized;
using System.Net.Http;
using System.Collections.Generic;
using System;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Orders;
using System.Text;
using Nop.Services.Customers;
using System.Linq;
using Nop.Web.Factories;
using Nop.Services.Directory;
using System.Globalization;

namespace NopValley.Plugin.Payments.Nmi.Controllers
{
    public class PaymentNmiPublicController : Controller
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;
        private const string NMI_3DS_POST_URL = "https://secure.networkmerchants.com/api/transact.php";
        private readonly ICustomerService _customerService;
        private readonly OrderSettings _orderSettings;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IAddressService _addressService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IEncryptionService _encryptionService;

        public PaymentNmiPublicController(IHttpContextAccessor httpContextAccessor, ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            IPermissionService permissionService, IStoreContext storeContext, INotificationService notificationService,
            IPaymentPluginManager paymentPluginManager,
            IGenericAttributeService genericAttributeService,
            IOrderService orderService,
            ICustomerService customerService,
            OrderSettings orderSettings,
            ICheckoutModelFactory checkoutModelFactory,
            IAddressService addressService,
            IStateProvinceService stateProvinceService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IEncryptionService encryptionService
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _settingService = settingService;
            _storeService = storeService;
            _workContext = workContext;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _notificationService = notificationService;
            _paymentPluginManager = paymentPluginManager;
            _genericAttributeService = genericAttributeService;
            _orderService = orderService;
            _customerService = customerService;
            _orderSettings = orderSettings;
            _checkoutModelFactory = checkoutModelFactory;
            _addressService = addressService;
            _stateProvinceService = stateProvinceService;
            _countryService = countryService;
            _currencyService = currencyService;
            _encryptionService = encryptionService;
        }

        

        [HttpPost]
        public async Task<IActionResult> EnterPaymentInfo(IFormCollection form)
        {            

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();            

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var nmiPaymentSettings = await _settingService.LoadSettingAsync<NmiPaymentSettings>(storeScope);

            //load payment method
            var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
               NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);

            var paymentMethod = await _paymentPluginManager
               .LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            var warnings = await paymentMethod.ValidatePaymentFormAsync(form);            

            foreach (var warning in warnings)
                ModelState.AddModelError("", warning);
            if (ModelState.IsValid)
            {
                //get payment info
                //var paymentInfo = await paymentMethod.GetPaymentInfoAsync(form);


                ////save settings
                //nmiPaymentSettings.Username = model.Username;
                //nmiPaymentSettings.Password = model.Password;
                //nmiPaymentSettings.UseUsernamePassword = model.UseUsernamePassword;
                //nmiPaymentSettings.AllowCustomerToSaveCards = model.AllowCustomerToSaveCards;
                //nmiPaymentSettings.AdditionalFee = model.AdditionalFee;
                //nmiPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
                //nmiPaymentSettings.SecurityKey = model.SecurityKey;
                //nmiPaymentSettings.CollectJsTokenizationKey = model.CollectJsTokenizationKey;
                //nmiPaymentSettings.TransactMode = (TransactMode)model.TransactModeId;


                return View("~/Plugins/Payments.Nmi/Views/PaymentInfo3DSECURE.cshtml");
            }

            return View("~/Plugins/Payments.Nmi/Views/Configure.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCreditCard(IFormCollection form)
        {      

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();


            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var nmiPaymentSettings = await _settingService.LoadSettingAsync<NmiPaymentSettings>(storeScope);

            //load payment method
            var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
               NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);

            var paymentMethod = await _paymentPluginManager
               .LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id);
            if (paymentMethod == null)
                return RedirectToRoute("CheckoutPaymentMethod");

            var warnings = await paymentMethod.ValidatePaymentFormAsync(form);

            foreach (var warning in warnings)
                ModelState.AddModelError("", warning);
            if (ModelState.IsValid)
            {
                //get payment info
                //var paymentInfo = await paymentMethod.GetPaymentInfoAsync(form);


                //save settings
              


                return View("~/Plugins/Payments.Nmi/Views/PaymentInfo3DSECURE.cshtml");
            }

            return View("~/Plugins/Payments.Nmi/Views/Configure.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> SaveThreeDSInfo(bool is3DS)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            await _genericAttributeService.SaveAttributeAsync<bool>(customer, "Is3DS", is3DS, store.Id);          

            return Json(new { response = is3DS });
        }

        public async Task<IActionResult> PaymentResult(string orderId)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var nmiPaymentSettings = await _settingService.LoadSettingAsync<NmiPaymentSettings>(storeScope);


            var request = _httpContextAccessor.HttpContext.Request;

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
               NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);

            var order = await _orderService.GetOrderByIdAsync(Convert.ToInt32(orderId));
            var customerAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            //customer currency
            //var currencyTmp = await _currencyService.GetCurrencyByIdAsync(
              //  await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults. .CurrencyIdAttribute, store.Id));

            var currencyTmp = await _currencyService.GetCurrencyByIdAsync(customer.CurrencyId ?? 0);
            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            var customerCurrency = currencyTmp != null && currencyTmp.Published ? currencyTmp : currentCurrency;

            var paymentInfoModel = new PaymentInfoModel();
            paymentInfoModel.OrderId = Convert.ToInt32(orderId);
            paymentInfoModel.CardNumber = _encryptionService.DecryptText(order.CardNumber);

            //expiry date
            var cardExpirationMonthDecrypted = _encryptionService.DecryptText(order.CardExpirationMonth);
            if (!string.IsNullOrEmpty(cardExpirationMonthDecrypted) && cardExpirationMonthDecrypted != "0")
                paymentInfoModel.ExpireMonth = cardExpirationMonthDecrypted;
            var cardExpirationYearDecrypted = _encryptionService.DecryptText(order.CardExpirationYear);
            if (!string.IsNullOrEmpty(cardExpirationYearDecrypted) && cardExpirationYearDecrypted != "0")
                paymentInfoModel.ExpireYear = cardExpirationYearDecrypted;


            //paymentInfoModel.ExpireMonth = order.CardExpirationMonth;
            //paymentInfoModel.ExpireYear = order.CardExpirationYear;
            paymentInfoModel.CardCode = _encryptionService.DecryptText(order.CardCvv2);

            paymentInfoModel.Amount = order.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture);
            paymentInfoModel.Currency = customerCurrency.CurrencyCode; //
            paymentInfoModel.Email = customer.Email;

            paymentInfoModel.Phone = customerAddress.PhoneNumber;
            paymentInfoModel.City = customerAddress.City;

            var state = await _stateProvinceService.GetStateProvinceByIdAsync(customerAddress.StateProvinceId ?? 0);

            paymentInfoModel.State = state?.Abbreviation;
            paymentInfoModel.Address1 = customerAddress.Address1;           

            var country = await _countryService.GetCountryByIdAsync(customerAddress.CountryId ?? 0);

            paymentInfoModel.Country = country?.TwoLetterIsoCode;

            paymentInfoModel.FirstName = customerAddress.FirstName;

            paymentInfoModel.LastName = customerAddress.LastName;
            paymentInfoModel.PostalCode = customerAddress.ZipPostalCode;

            paymentInfoModel.SecurityKey = nmiPaymentSettings.SecurityKey;
            paymentInfoModel.CheckoutKey = nmiPaymentSettings.CheckoutKey;
            paymentInfoModel.CollectJsTokenizationKey = nmiPaymentSettings.CollectJsTokenizationKey;
            // paymentInfoModel.

            return View("~/Plugins/Payments.Nmi/Views/Payment3DS.cshtml", paymentInfoModel);

        }

        [HttpPost]
        //public async Task<IActionResult> NMIAPI(string cavv, string xid, string eci, string cardHolderAuth, string threeDsVersion, string directoryServerId)
        public async Task<IActionResult> NMIAPI([FromBody] NMIPaymentModel paymentModel)
        {
            
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
               NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);

            var orderId = Convert.ToInt32(paymentModel.OrderId);

            var postResponse = string.Empty;
            var postResponseText = string.Empty;

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var nmiPaymentSettings = await _settingService.LoadSettingAsync<NmiPaymentSettings>(storeScope);

            //post data
            if (paymentModel.CardHolderAuth == "verified")
            {
                try
                {
                    var cardExpMonth = paymentModel.CardExpMonth.Length == 1 ? ("0" + paymentModel.CardExpMonth) : paymentModel.CardExpMonth;
                    using (var client = new HttpClient())
                    {
                        var values = new List<KeyValuePair<string, string>>();
                        values.Add(new KeyValuePair<string, string>("security_key", nmiPaymentSettings.SecurityKey));
                        values.Add(new KeyValuePair<string, string>("ccnumber", paymentModel.CardNumber));
                        values.Add(new KeyValuePair<string, string>("ccexp", cardExpMonth + paymentModel.CardExpYear));
                        values.Add(new KeyValuePair<string, string>("amount", paymentModel.Amount));

                        values.Add(new KeyValuePair<string, string>("email", paymentModel.Email));
                        values.Add(new KeyValuePair<string, string>("phone", paymentModel.Phone));

                        values.Add(new KeyValuePair<string, string>("city", paymentModel.City));
                        values.Add(new KeyValuePair<string, string>("address1", paymentModel.Address1));

                        values.Add(new KeyValuePair<string, string>("country", paymentModel.Country));
                        values.Add(new KeyValuePair<string, string>("first_name", paymentModel.FirstName));

                        values.Add(new KeyValuePair<string, string>("last_name", paymentModel.LastName));
                        values.Add(new KeyValuePair<string, string>("zip", paymentModel.PostalCode));

                        values.Add(new KeyValuePair<string, string>("cavv", paymentModel.Cavv));
                        values.Add(new KeyValuePair<string, string>("xid", paymentModel.Xid));

                        values.Add(new KeyValuePair<string, string>("eci", paymentModel.Eci));
                        values.Add(new KeyValuePair<string, string>("cardholder_auth", paymentModel.CardHolderAuth));

                        values.Add(new KeyValuePair<string, string>("three_ds_version", paymentModel.ThreeDsVersion));
                        //values.Add(new KeyValuePair<string, string>("directory_server_id", "string value"));

                        var order = await _orderService.GetOrderByIdAsync(orderId);

                        var content = new FormUrlEncodedContent(values);

                        var response = await client.PostAsync(NMI_3DS_POST_URL, content);                      

                        var responseValues = ExtractResponseValues(response.Content.ReadAsStringAsync().Result);

                        var responseValue = responseValues["response"];
                        postResponse = responseValue;
                        postResponseText = responseValues["responsetext"];

                        if (responseValue == "1")
                        {   
                            order.PaymentStatus = PaymentStatus.Paid;
                            order.OrderStatus = OrderStatus.Processing;                              
                        }

                        //order note 
                        var sb = new StringBuilder();
                        sb.AppendLine("NMI 3DSecure Payment:");
                        sb.AppendLine("transactionid :  " + responseValues["transactionid"]);

                        await _orderService.InsertOrderNoteAsync(new OrderNote
                        {
                            OrderId = order.Id,
                            Note = sb.ToString(),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                       
                        await _orderService.UpdateOrderAsync(order);  
                    }

                }
                catch (Exception iexc)
                {
                    //_logger.InsertLog(Core.Domain.Logging.LogLevel.Debug, "est-Approved-innerexc", "oid:" + iexc.ToString());
                }
              
            }

            return Json(
                        new
                        {
                            response = postResponse,
                            responseText = postResponseText
                        });

        }

        private NameValueCollection ExtractResponseValues(string response)
        {
            var responseValues = new NameValueCollection();
            var split = response.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var parts in split
                .Select(s => s.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(parts => parts.Length == 2))
            {
                responseValues.Add(parts[0], parts[1]);
            }

            return responseValues;
        }


        public virtual async Task<IActionResult> Completed(int? orderId)
        {
            //validation
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            Order order = null;
            if (orderId.HasValue)
            {
                //load order by identifier (if provided)
                order = await _orderService.GetOrderByIdAsync(orderId.Value);
            }
            if (order == null)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                order = (await _orderService.SearchOrdersAsync(storeId: store.Id,
                customerId: customer.Id, pageSize: 1))
                    .FirstOrDefault();
            }
            if (order == null || order.Deleted || customer.Id != order.CustomerId)
            {
                return RedirectToRoute("Homepage");
            }

            //disable "order completed" page?
            if (_orderSettings.DisableOrderCompletedPage)
            {
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }

            //model
            var model = await _checkoutModelFactory.PrepareCheckoutCompletedModelAsync(order);
            return View("~/Plugins/Payments.Nmi/Views/Completed.cshtml", model);
        }
    }
}