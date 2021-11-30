using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArumModels.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotnetAWSBeanstalkBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        // GET: api/<ShopController>
        [HttpGet]
        public IEnumerable<Shop> Get()
        {
            return Shop.GetShops();
        }

        // GET api/<ShopController>/5
        [HttpGet("{id:int}")]
        public Shop Get(int id)
        {
            return Shop.GetById(id);
        }

        [HttpGet("{id:int}/getAllMenus")]
        public Shop GetMassiveShopInfo(int id)
        {
            return Shop.GetAllCategoriesAndMenus(id);
        }

        [HttpGet("NearBy/{N:int}/{latitude:double}/{longitude:double}")]
        public IEnumerable<Shop> GetNearbyShops(int N, double latitude, double longitude)
        {
            return Shop.GetNearbyShops(N, latitude, longitude);
        }

        // POST api/<ShopController>
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
