using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        DbContext context;
        DbSet<T> table;
        public GenericRepository(DbContext context)
        {
            this.context = context;
            table = context.Set<T>();
        }
        public async Task AddAsync(T entity) => await table.AddAsync(entity);

        public async Task DeleteAsync(T entity) => await Task.Factory.StartNew(() => table.Remove(entity));

        public IEnumerable<T> GetAll() => table;

        public async Task<T> GetAsync(int id) => await table.FindAsync(id);

        public async Task SaveChangesAsync() => await context.SaveChangesAsync();

        public async Task UpdateAsync(T entity) => await Task.Factory.StartNew(() => table.Update(entity));
    }
}
