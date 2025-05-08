using M3alam.Common.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace M3alam.Web.Controllers
{
    [ApiController]  
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class TestController : ApiController   // renamed from “TestControllee”
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("Auth Working!");
        }
        [HttpPost]
        public ActionResult<bool> create(CreateProviderAccountDto DTO)
        {
            return Mediator.send{CreateProviderAccountCommand : {
                dto: Dto
            }};
        }
    }
}
