using Microsoft.EntityFrameworkCore;
using RithV.Services.CORE.API.Domain;
using RithV.Services.CORE.API.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RithV.Services.CORE.API.Repositories
{
    public class CustomerRepository
        : ICustomerRepository
    {
        private readonly COREAPIContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public CustomerRepository(COREAPIContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Customer Add(Customer customer)
        {
            
                return _context.Customers
                    .Add(customer)
                    .Entity;
           
        }

        public Customer Update(Customer customer)
        {
            return _context.Customers
                    .Update(customer)
                    .Entity;
        }

        public async Task<Customer> FindAsync(Int64 identity)
        {
            var buyer = await _context.Customers
                .Where(b => b.Id == identity)
                .SingleOrDefaultAsync();

            return buyer;
        }

    }
}
