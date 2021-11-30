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
    public class IngredientController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Ingredient> GetIngredients(int menuId)
        {
            return Ingredient.GetIngredients(menuId);
        }

        [HttpGet("~/api/[controller]/{ingredientId:int}")]
        [HttpGet("{id}")]
        public Ingredient GetIngredient(int ingredientId)
        {
            return Ingredient.GetIngredient(ingredientId);
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
