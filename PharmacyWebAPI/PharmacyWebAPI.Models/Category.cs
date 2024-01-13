namespace PharmacyWebAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "New Category";
        public string ImageURL { get; set; } = string.Empty;
        public string ImageId { get; set; } = string.Empty;
    }
}