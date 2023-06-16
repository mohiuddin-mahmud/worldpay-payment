using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace NopValley.Plugin.Payments.Nmi.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //get language pattern
            //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
            //use it only for URLs of pages that the user can go to
            

            endpointRouteBuilder.MapControllerRoute("Plugins.Payments.Nmi.PaymentResult",
                "Plugins/PaymentNmiPublic/PaymentResult",
                new { controller = "PaymentNmiPublic", action = "PaymentResult" });

            endpointRouteBuilder.MapControllerRoute("Plugins.Payments.Nmi.NMIAPI",
                "Plugins/PaymentNmiPublic/NMIAPI",
                new { controller = "PaymentNmiPublic", action = "NMIAPI" });

            endpointRouteBuilder.MapControllerRoute(name: "NMIPaymentCompleted",
                pattern: $"nmicheckout/completed/{{orderId:int?}}",
                defaults: new { controller = "PaymentNmiPublic", action = "Completed" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 100;
    }
}