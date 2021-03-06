using RithV.Services.CORE.API.Domain;
using System;
using System.Threading.Tasks;

namespace RithV.Services.CORE.API.Domain
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Customer Add(Customer buyer);
        Customer Update(Customer buyer);
        Task<Customer> FindAsync(Int64 Id);
    }
}
