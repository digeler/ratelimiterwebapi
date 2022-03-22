using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyAppController : ControllerBase
    {
        private readonly ILogger<MyAppController> _logger;

        public MyAppController(ILogger<MyAppController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("/")]
        public Task<IActionResult> Get()
        {
            var customervalue = HttpContext.Request.Query["clientid"].ToString();
            if (HttpContext.Request.Query.Keys.Contains("clientid"))
            {
                _logger.LogInformation("sending ok");
                return Task.FromResult<IActionResult>(Ok("your request has been  served"));
            }
            _logger.LogInformation("sending 401");
            return Task.FromResult<IActionResult>(StatusCode(401));
        }
    }
}