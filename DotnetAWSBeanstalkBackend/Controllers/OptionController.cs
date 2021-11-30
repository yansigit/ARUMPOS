using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArumModels.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotnetAWSBeanstalkBackend.Controllers
{
    [Route("api/Shop/{shopId:int}/Category/{categoryId:int}/Menu/{menuId:int}/[controller]/")]
    [ApiController]
    public class OptionController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Option> GetOptions(int menuId)
        {
            return Option.GetOptions(menuId);
        }

        [HttpGet("~/api/[controller]/{optionId:int}")]
        [HttpGet("{optionId}")]
        public Option GetOption(int optionId)
        {
            return Option.GetOption(optionId);
        }

        // // POST api/<ShopController>
        // [HttpPost]
        // public void Post([FromBody] string value)
        // {
        // }
        //
        // // PUT api/<ShopController>/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody] string value)
        // {
        // }
        //
        // // DELETE api/<ShopController>/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}
