namespace PharmacyWebAPI.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task SetOrderId(int OrderId, List<OrderDetail> details)
        {
            foreach (var d in details)
            {
                d.OrderId = OrderId;
            }
            await _context.OrderDetail.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }
    }
}