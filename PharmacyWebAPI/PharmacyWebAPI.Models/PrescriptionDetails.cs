namespace PharmacyWebAPI.Models
{
    public class PrescriptionDetails
    {
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Dose { get; set; }
        public int Dosage { get; set; }
        public bool BeforeAfterMeal { get; set; }

        [ForeignKey("Prescription")]
        public int PrescriptionId { get; set; }

        public Prescription Prescription { get; set; }

        [ForeignKey("Drug")]
        public int DrugId { get; set; }

        public Drug Drug { get; set; }
    }
}