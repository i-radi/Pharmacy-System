using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyWebAPI.Models.Dto
{
    public class ResponseURLsDto
    {
        public string DomainName { get; set; }
        public string SuccessUrl { get; set; }
        public string FaildUrl { get; set; }
        public string token { get; set; }
    }
}