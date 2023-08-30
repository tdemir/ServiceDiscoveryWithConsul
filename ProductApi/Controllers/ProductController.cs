using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {

        // GET: api/values
        [HttpGet]
        public IEnumerable<Models.Product> Get()
        {
            return GetProducts();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Models.Product Get(int id)
        {
            return GetProducts().FirstOrDefault(x => x.Id == id);
        }


        private List<Models.Product> GetProducts()
        {
            var apiNumber = string.IsNullOrWhiteSpace(Constants.API_NUMBER) ? string.Empty : " - "+ Constants.API_NUMBER;

            var list = new List<Models.Product>();
            list.Add(new Models.Product() { Id = 1, Name = "Keyboard" + apiNumber, Price = 15 });
            list.Add(new Models.Product() { Id = 2, Name = "CPU" + apiNumber, Price = 150 });
            list.Add(new Models.Product() { Id = 3, Name = "Mouse" + apiNumber, Price = 20 });
            list.Add(new Models.Product() { Id = 4, Name = "Monitor" + apiNumber, Price = 150 });


            return list;
        }


    }
}

