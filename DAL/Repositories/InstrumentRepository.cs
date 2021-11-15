using DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class InstrumentRepository : GenericRepository<Instrument>
    {
        public InstrumentRepository(FinancialContext context) : base(context)
        {

        }
    }
}
