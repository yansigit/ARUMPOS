using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArumModels.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotnetAWSBeanstalkBackend.Controllers
{
    [Route("api/Shop/{shopId:int}/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        // Shop ID -> Categories
        // GET api/Shop/12345/Category/
        [HttpGet]
        public IEnumerable<Category> GetCategories(int shopId)
        {
            return Category.GetCategories(shopId);
        }

        // GET api/Shop/{shopId}/Category/{categoryId}
        [HttpGet("{categoryId:int}")]
        [HttpGet("~/api/[controller]/{categoryId:int}")]
        public Category Get(int categoryId)
        {
            return Category.GetCategory(categoryId);
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
