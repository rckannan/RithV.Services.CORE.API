using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RithV.Services.CORE.API.Infra;

namespace RithV.Services.CORE.API.Controllers
{
    //[Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET: /<controller>/

        [HttpGet]
        [Route(Approute.BBFMC.home_index)]
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
