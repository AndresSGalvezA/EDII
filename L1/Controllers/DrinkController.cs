using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using L1.Models;
using L1.Helpers;

namespace L1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DrinkController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Drink> Get()
        {
            Data.Instance.Items = Data.Instance.Tree.InOrder(Data.Instance.Tree.Root, Data.Instance.Items);
            return Data.Instance.Items;
        }

        [HttpGet("{id}", Name = "Get")]
        public object Get(string id)
        {
            var drink = new Drink { Name = id };
            return Data.Instance.Tree.Search(drink)? Data.Instance.Tree.Seeker(Data.Instance.Tree.Root, drink) : null;
        }

        [HttpPost]
        public void Post([FromBody] Drink drink)
        {
            Data.Instance.Tree.Insert(drink);
        }
    }
}
