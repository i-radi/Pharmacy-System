namespace PharmacyWebAPI.DataAccess.Repository
{
    public class DrugRepository : Repository<Drug>, IDrugRepository
    {
        private readonly ApplicationDbContext _context;

        public DrugRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}