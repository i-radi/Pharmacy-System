using PharmacyWebAPI.Models;
using PharmacyWebAPI.Models.Dto;

namespace PharmacyWebAPI.DataAccess.Repository
{
    public class PresciptionDetailsRepository : Repository<PrescriptionDetails>, IPrescriptionDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public PresciptionDetailsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task SetPresciptionId(int PresciptionId, List<PrescriptionDetails> details)
        {
            foreach (var d in details)
            {
                d.PrescriptionId = PresciptionId;
            }
            await _context.PrescriptionDetail.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public List<OrderDetail> PrescriptionDetailsToOrderDetails(List<PrescriptionDetails> prescriptionDetails)
        {
            var details = new List<OrderDetail>();
            foreach (var item in prescriptionDetails)
            {
                var drug = _context.Drugs.FirstOrDefault(i => i.Id == item.DrugId);
                details.Add(new OrderDetail
                {
                    Count = 1,
                    DrugId = item.DrugId,
                    Price = drug.Price
                });
            }
            return details;
        }
    }
}