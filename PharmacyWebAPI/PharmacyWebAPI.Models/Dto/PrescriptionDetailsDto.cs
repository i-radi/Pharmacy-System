using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace PharmacyWebAPI.Models.Dto
{
    public class PrescriptionDetailsDto
    {
        public int Id { get; set; }
        public string Comment { get; set; } = "Empty";
        public int Dose { get; set; }
        public int Dosage { get; set; }
        public bool BeforeAfterMeal { get; set; }
        public int DrugId { get; set; }
    }
}