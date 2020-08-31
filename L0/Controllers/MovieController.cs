using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using L0.Helpers;

namespace L0.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Movie> Get()
        {
            if (Data.Instance.Movies.Count <= 10) 
                return Data.Instance.Movies;

            for (var i = 0; i < Data.Instance.Movies.Count - 10; i++)
            {
                Data.Instance.Movies.Dequeue();
            }

            return Data.Instance.Movies;
        }

        [HttpPost]
        public Movie Post([FromBody]Movie movie)
        {
            Data.Instance.Movies.Enqueue(movie);
            return movie;
        }
    }
}
