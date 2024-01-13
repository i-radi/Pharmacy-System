using PharmacyWebAPI.Models.Enums;

namespace PharmacyWebAPI.Models.Dto
{
    public class PostDrugDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DosageForm DosageForm { get; set; }
        public string DosageStrength { get; set; }
        public List<string> SideEffects { get; set; }
        public string Contraindications { get; set; }
        public string Description { get; set; }
        public PregnancyCategory PregnancyCategory { get; set; }
        public double Price { get; set; }
        public int ManufacturerId { get; set; }
        public int CategoryId { get; set; }

        [ValidateNever]
        public IEnumerable<Manufacturer>? Manufacturers { get; set; }

        [ValidateNever]
        public IEnumerable<Category>? Categories { get; set; }
    }
}