using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopValley.Plugin.Payments.Nmi.Models
{
    public class NMIPaymentModel
    {
        public NMIPaymentModel()
        {
           
        }

        [JsonProperty("cavv")]
        public string Cavv { get; set; }

        [JsonProperty("xid")]
        public string Xid { get; set; }

        [JsonProperty("eci")]
        public string Eci { get; set; }

        [JsonProperty("cardHolderAuth")]
        public string CardHolderAuth { get; set; }

        [JsonProperty("threeDsVersion")]
        public string ThreeDsVersion { get; set; }

        [JsonProperty("directoryServerId")]
        public string DirectoryServerId { get; set; }

        [JsonProperty("cardNumber")]
        public string CardNumber { get; set; }

        [JsonProperty("cardExpMonth")]
        public string CardExpMonth { get; set; }

        [JsonProperty("cardExpYear")]
        public string CardExpYear { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("address1")]
        public string Address1 { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("orderid")]
        public string OrderId { get; set; }

    }
}