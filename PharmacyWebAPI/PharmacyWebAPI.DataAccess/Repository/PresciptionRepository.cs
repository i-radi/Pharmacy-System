using PharmacyWebAPI.Models;

namespace PharmacyWebAPI.DataAccess.Repository
{
    public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}