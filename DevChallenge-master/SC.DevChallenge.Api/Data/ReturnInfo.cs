using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SC.DevChallenge.Api.Data
{
    [Serializable]
    public class ReturnInfo
    {
        public double Price { get; set; }
        public DateTime Date { get; set; }
    }
}
