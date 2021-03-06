using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RithV.Services.CORE.API.Commands;
using RithV.Services.CORE.API.Infra;
using RithV.Services.CORE.API.Queries;

namespace RithV.Services.CORE.API.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICustomerQueries _orderQueries;
        private readonly ILogger<CustomerController> _logger;
        //private readonly IOptions<approute> _approute;


        public CustomerController(ILogger<CustomerController> logger, IMediator mediator, ICustomerQueries orderQueries )
        {
            _logger = logger;
            _mediator = mediator;
            _orderQueries = orderQueries;
            //_approute = approute;
        }

        //[Route("{orderId:int}")]
        [Route(Approute.BBFMC.customer_getOrder)]
        //[Route(approute.customer_getOrder)]
        [HttpGet()]
        [ProducesResponseType(typeof(CustomerDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetOrderAsync()
        {
            try
            {
                //Todo: It's good idea to take advantage of GetOrderByIdQuery and handle by GetCustomerByIdQueryHandler
                //var order customer = await _mediator.Send(new GetOrderByIdQuery(orderId));
                var order = await _orderQueries.GetCustomersAsync();

                return Ok(order);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [Route(Approute.BBFMC.customer_create)]
        //[Route(Approute.customer_create)]
        [HttpPost]
        public async Task<ActionResult<bool>> CreateCustomerAsync([FromBody] CustomerDomainCommand customerDomainCommand)
        {
            _logger.LogInformation(
                "----- Sending command: {CommandName} - {Name}: {CommandId} ({@Command})",
                customerDomainCommand.GetGenericTypeName(),
                nameof(customerDomainCommand.UserName),
                customerDomainCommand.UserId,
                customerDomainCommand);

            return await _mediator.Send(customerDomainCommand);
        }
    }
}
