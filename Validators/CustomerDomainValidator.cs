using FluentValidation;
using Microsoft.Extensions.Logging;
using RithV.Services.CORE.API.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RithV.Services.CORE.API.Validators
{
   
    public class CustomerDomainValidator : AbstractValidator<CustomerDomainCommand>
    {
        public CustomerDomainValidator(ILogger<CustomerDomainValidator> logger)
        {
            RuleFor(command => command.UserName).NotEmpty();
            RuleFor(command => command.FullName).NotEmpty(); ;
            RuleFor(command => command.BirthDate).NotEmpty().Must(BeValidExpirationDate).WithMessage("Please specify a valid date of birth"); 
            //RuleFor(command => command.OrderItems).Must(ContainOrderItems).WithMessage("No order items found");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        private bool BeValidExpirationDate(DateTime dateTime)
        {
            return dateTime <= DateTime.UtcNow;
        }

        //private bool ContainOrderItems(IEnumerable<OrderItemDTO> orderItems)
        //{
        //    return orderItems.Any();
        //}
    }
}
