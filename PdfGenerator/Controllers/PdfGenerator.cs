using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PdfGenerator.Models;
using Swashbuckle.Examples.Auto;

namespace PdfGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/pdf")]

    public class PdfGeneratorController : ControllerBase
    {


       
        [HttpPost]
           public IActionResult Post([FromQuery]   string templateName, [FromBody] FareRuleData data)
        {
            return Ok(data);
        }


    }
}
