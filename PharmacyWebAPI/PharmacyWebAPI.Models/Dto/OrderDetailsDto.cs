using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace PharmacyWebAPI.Models.Dto
{
    public class OrderDetailsDto
    {
        public int DrugId { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}