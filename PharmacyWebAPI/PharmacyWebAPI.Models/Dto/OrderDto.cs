using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace PharmacyWebAPI.Models.Dto
{
    public class OrderDto
    {
        public string UserId { get; set; }
    }
}