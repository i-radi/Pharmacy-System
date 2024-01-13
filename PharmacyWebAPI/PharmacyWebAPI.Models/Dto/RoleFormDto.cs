using System.ComponentModel.DataAnnotations;

namespace PharmacyWebAPI.Models.Dto
{
    public class RoleFormDto
    {
        [Required, StringLength(256)]
        public string Name { get; set; }
    }
}