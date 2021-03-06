using MediatR;
using System;

namespace RithV.Services.CORE.API.Events
{
    public class CustomerDomainEvent : INotification
    {
        public Int64 UserId { get; }
        public string UserName { get; }
        public string FullUserName { get; }
        public DateTime Date_Birth { get; }
        //public Customer customer { get; } 

        public CustomerDomainEvent(Int64 userId, string userName, string fullName,   DateTime DOB)
        { 
            UserId =  userId;
            UserName =  userName;
            FullUserName = fullName;
            Date_Birth = DOB;
            //customer = cust; 
        }
    }
}
