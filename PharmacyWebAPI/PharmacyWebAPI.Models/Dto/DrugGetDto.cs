using PharmacyWebAPI.Models.Enums;

namespace PharmacyWebAPI.Models.Dto
{
    public class DrugGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DosageForm DosageForm { get; set; }
        public string DosageStrength { get; set; }
        public List<string> SideEffects { get; set; }
        public string Contraindications { get; set; }
        public string Description { get; set; }
        public PregnancyCategory PregnancyCategory { get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
        public string ImageURL { get; set; }
        public string ImageId { get; set; }
        public int ManufacturerId { get; set; }
        public int CategoryId { get; set; }
    }
}