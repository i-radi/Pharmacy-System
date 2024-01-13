using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyWebAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int Stock { get; set; } = 20;
        public double Price { get; set; } = GenerateRandomNumber();

        [ForeignKey("Drug")]
        public int DrugId { get; set; }

        public Drug Drug { get; set; }

        private static double GenerateRandomNumber()
        {
            Random random = new();
            var number = random.NextDouble() * (100 - 1) + 1;
            var result = double.Parse(number.ToString("00.00"));
            return result;
        }
    }
}