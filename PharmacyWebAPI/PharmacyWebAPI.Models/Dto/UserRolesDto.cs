namespace PharmacyWebAPI.Models.Dto
{
    public class UserRolesDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}