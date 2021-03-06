using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RithV.Services.CORE.API.Infra
{
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error)
            : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
