using MediatR;
using Microsoft.Extensions.Logging;
using RithV.Services.CORE.API.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RithV.Services.CORE.API.Commands
{
    public class CreateCustomerCommandHandler
        : IRequestHandler<CustomerDomainCommand, bool>
    {
        private readonly ICustomerRepository _customerRepository; 
        private readonly IMediator _mediator; 
        private readonly ILogger<CreateCustomerCommandHandler> _logger;

        // Using DI to inject infrastructure persistence Repositories
        public CreateCustomerCommandHandler(IMediator mediator, 
            ICustomerRepository customerRepository, 
            ILogger<CreateCustomerCommandHandler> logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository)); 
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CustomerDomainCommand message, CancellationToken cancellationToken)
        {
          
            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate 
            var order = new Customer(message.Id, message.UserId, message.UserName,  message.FullName, message.BirthDate);

            
            _logger.LogInformation("----- Creating Customer - Customer: {@Order}", order);

            _customerRepository.Add(order);

            return await _customerRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);
        }
    }

    // Use for Idempotency in Command process
    //public class CreateCustomerIdentifiedCommandHandler : IdentifiedCommandHandler<CreateOrderCommand, bool>
    //{
    //    public CreateCustomerIdentifiedCommandHandler(
    //        IMediator mediator,
    //        IRequestManager requestManager,
    //        ILogger<IdentifiedCommandHandler<CreateOrderCommand, bool>> logger)
    //        : base(mediator, requestManager, logger)
    //    {
    //    }

    //    protected override bool CreateResultForDuplicateRequest()
    //    {
    //        return true;                // Ignore duplicate requests for creating order.
    //    }
    //}
}
