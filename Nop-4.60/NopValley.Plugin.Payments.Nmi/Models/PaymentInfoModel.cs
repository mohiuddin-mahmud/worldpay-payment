using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopValley.Plugin.Payments.Nmi.Models
{
    public class PaymentInfoModel
    {
        public PaymentInfoModel()
        {
            StoredCards = new List<SelectListItem>();
            ExpireMonths = new List<SelectListItem>();
            ExpireYears = new List<SelectListItem>();
        }

        // These properties are only used to display label on the payment info screen
        [NopResourceDisplayName("Payment.CardNumber")]
        public string CardNumber { get; set; }

        [NopResourceDisplayName("Payment.ExpirationDate")]
        public string ExpireMonth { get; set; }

        [NopResourceDisplayName("Payment.ExpirationDate")]
        public string ExpireYear { get; set; }

        public IList<SelectListItem> ExpireMonths { get; set; }

        public IList<SelectListItem> ExpireYears { get; set; }

        [NopResourceDisplayName("Payment.CardCode")]
        public string CardCode { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Nmi.SaveCustomer")]
        public string SaveCustomer { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Nmi.Fields.StoredCard")]
        public string StoredCardId { get; set; }
        public IList<SelectListItem> StoredCards { get; set; }


        public string Token { get; set; }

        public bool IsGuest { get; set; }

        public bool AllowCustomerToSaveCards { get; set; }

        public int OrderId { get; set; }

        public bool Is3DS { get; set; }
        public string Currency { get; set; }

        public string Amount { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address1 { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PostalCode { get; set; }

        public string SecurityKey { get; set; }
        public string CheckoutKey { get; set; }
        public string CollectJsTokenizationKey { get; set; }

    }
}