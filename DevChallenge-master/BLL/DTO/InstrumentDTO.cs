using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class InstrumentDTO
    {
        public int Id { get; set; }
        public string Portfolio { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
    }
}
