using Dapper;
using Microsoft.Data.SqlClient;
using RithV.Services.CORE.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RithV.Services.CORE.API.Queries
{

    public interface ICustomerQueries
    {
        Task<Customer> GetCustomerAsync(int id);

        Task<CustomerDTO> GetCustomersAsync();
    }
    public class DataQuery : ICustomerQueries
    {
        private string _connectionString = string.Empty;
        public DataQuery(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        } 
         

        public async Task<Customer> GetCustomerAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<dynamic>(
                   @"select * from COREAPI.tblCustomer
                        WHERE o.Id=@id"
                        , new { id }
                    );

                if (result.AsList().Count == 0)
                    throw new KeyNotFoundException();

                return (Customer)result ;
            }
        }

        public async Task<CustomerDTO> GetCustomersAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<dynamic>(
                   @"select * from COREAPI.tblCustomer ");

                if (result.AsList().Count == 0)
                    throw new KeyNotFoundException();

                return MapCustomerItems(result);
            }
        }

        private CustomerDTO MapCustomerItems(dynamic result)
        {
           
            var order = new CustomerDTO
            {
                CustomerID = result[0].UserID,
                Name = result[0].Name,
                FullName = result[0].FullName,
                DOB = result[0].DateofBirth 
            }; 
            return order;
        }
    }

    public class CustomerDTO
    {
        public Int64 CustomerID { get; set; }
        public string Name { get; set; }
        public String FullName { get; set; }
        public DateTime DOB { get; set; }
    }
}
