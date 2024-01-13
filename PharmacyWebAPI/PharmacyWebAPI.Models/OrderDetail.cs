namespace PharmacyWebAPI.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        [ForeignKey("Drug")]
        public int DrugId { get; set; }

        public Drug Drug { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}