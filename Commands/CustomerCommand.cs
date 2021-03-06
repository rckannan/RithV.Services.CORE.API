using MediatR;
using RithV.Services.CORE.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace RithV.Services.CORE.API.Commands
{

    [DataContract]
    public class CustomerDomainCommand
       : IRequest<bool>
    {
        [DataMember]
        public Int64 Id { get; private set; }

        [DataMember]
        public Int64 UserId { get; private set; }

        [DataMember]
        public string UserName { get; private set; }

        [DataMember]
        public string FullName { get; private set; }

        [DataMember]
        public DateTime BirthDate { get; private set; }

        //[DataMember]
        //private readonly List<OrderItemDTO> _orderItems;

        //[DataMember]
        //public IEnumerable<Address> AddressItems => _orderItems;

        public CustomerDomainCommand()
        {
            //_orderItems = new List<OrderItemDTO>();
        }

        public CustomerDomainCommand( Int64 id, Int64 userId, string userName, string fullName, DateTime DOB ) : this()
        {
            Id = id;
            UserId = userId;
            UserName = userName;
            FullName = fullName;
            BirthDate = DOB; 
        } 
    }

    
}
