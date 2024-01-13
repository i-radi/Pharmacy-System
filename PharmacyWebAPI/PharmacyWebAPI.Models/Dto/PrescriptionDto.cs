using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace PharmacyWebAPI.Models.Dto
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public DateTime CreationDateTime { get; set; } = DateTime.Now;
        public bool dispensing { get; set; } = false;
        public string PatientId { get; set; }

        [ValidateNever]
        public User Patient { get; set; }

        public string DoctorId { get; set; }

        [ValidateNever]
        public User Doctor { get; set; }

        [ValidateNever]
        public IEnumerable<User> Patients { get; set; }
    }
}