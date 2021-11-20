using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Context
{
    public class FinancialContext : DbContext
    {
        public DbSet<Instrument> Instruments { get; set; }
        public FinancialContext(DbContextOptions<FinancialContext> option) : base(option)
        {
            Database.EnsureCreated();
        }
    }
}
