namespace PharmacyWebAPI.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IDrugRepository Drug { get; private set; }

        public IManufacturerRepository Manufacturer { get; private set; }

        public ICategoryRepository Category { get; private set; }

        public IUserRepository User { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }

        public IPrescriptionRepository Prescription { get; private set; }

        public IPrescriptionDetailsRepository PrescriptionDetails { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Drug = new DrugRepository(_context);
            Manufacturer = new ManufacturerRepository(_context);
            Category = new CategoryRepository(_context);
            User = new UserRepository(_context);
            Order = new OrderRepository(_context);
            OrderDetail = new OrderDetailRepository(_context);
            Prescription = new PrescriptionRepository(_context);
            PrescriptionDetails = new PresciptionDetailsRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}