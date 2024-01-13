using Microsoft.EntityFrameworkCore;
using PharmacyWebAPI.Models.Dto;
using Stripe.Checkout;

namespace PharmacyWebAPI.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

        void UpdateStripePaymentID(int id, string paymentItentId);

        double GetTotalPrice(List<OrderDetail> Drugs);

        Task<Session> StripeSetting(Order order, List<OrderDetail> orderDetails, ResponseURLsDto URLs);

        SessionCreateOptions GenerateOptions(int OrderId, ResponseURLsDto URLs);

        Task SetOptionsValues(SessionCreateOptions options, List<OrderDetail> orderDetails);
    }
}