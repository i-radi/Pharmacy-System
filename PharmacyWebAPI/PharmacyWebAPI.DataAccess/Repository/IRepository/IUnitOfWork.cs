namespace PharmacyWebAPI.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IDrugRepository Drug { get; }
        IManufacturerRepository Manufacturer { get; }
        ICategoryRepository Category { get; }
        IUserRepository User { get; }
        IOrderRepository Order { get; }
        IOrderDetailRepository OrderDetail { get; }
        IPrescriptionRepository Prescription { get; }
        IPrescriptionDetailsRepository PrescriptionDetails { get; }

        Task<int> SaveAsync();
    }
}