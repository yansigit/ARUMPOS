using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArumModels.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotnetAWSBeanstalkBackend.Controllers
{
    [Route("api/Shop/{shopId:int}/Category/{categoryId:int}/[controller]/")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        // GET: api/Shop/12345/Category/1/Menu
        [HttpGet]
        public IEnumerable<Menu> GetMenus(int categoryId)
        {
            return Menu.GetMenus(categoryId);
        }

        [HttpGet("~/api/[controller]/{menuId:int}")]
        [HttpGet("{menuId}")]
        public Menu GetMenu(int menuId)
        {
            return Menu.GetMenu(menuId);
        }

        //
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
