namespace PharmacyWebAPI.DataAccess.Repository
{
    public class ManufacturerRepository : Repository<Manufacturer>, IManufacturerRepository
    {
        private readonly ApplicationDbContext _context;

        public ManufacturerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}