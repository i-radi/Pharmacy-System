using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyWebAPI.Models.Dto
{
    public class CheckTokenDto
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }
}