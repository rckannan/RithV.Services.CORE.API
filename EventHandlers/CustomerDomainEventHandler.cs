using MediatR;
using Microsoft.Extensions.Logging;
using RithV.Services.CORE.API.Domain;
using RithV.Services.CORE.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RithV.Services.CORE.API.EventHandlers
{
    
    public class CustomerDomainEventHandler
                        : INotificationHandler<CustomerDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly ICustomerRepository _CustomerRepository;
        //private readonly IIdentityService _identityService;
       // private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public CustomerDomainEventHandler(
            ILoggerFactory logger,
            ICustomerRepository customerRepository 
          )
        {
            _CustomerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            //_identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            //_orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CustomerDomainEvent customerStartedEvent, CancellationToken cancellationToken)
        {
            
            var customer = await _CustomerRepository.FindAsync(customerStartedEvent.UserId);
            bool customerOriginallyExisted = (customer == null) ? false : true; 
          

            var customerUpdated = customerOriginallyExisted ?
                _CustomerRepository.Update(customer) :
                _CustomerRepository.Add(customer);

            await _CustomerRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);

        
            _logger.CreateLogger<CustomerDomainEventHandler>()
                .LogTrace("Customer {Id} has been updated named {UserName}",
                    customerUpdated.Id, customerStartedEvent.UserName);
        }
    }
}
