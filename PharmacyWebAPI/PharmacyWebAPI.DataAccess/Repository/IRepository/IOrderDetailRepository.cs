namespace PharmacyWebAPI.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task SetOrderId(int OrderId, List<OrderDetail> details);
    }
}