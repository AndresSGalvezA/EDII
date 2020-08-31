using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace L4.Models
{
    public class CipherRouteRequest : ICipherRequest<int>
    {
        public IFormFile File { get; set; }
        public int Key { get; set; }
        public int KeyColumns { get; set; }
        public string Name { get; set; }
        public string Direction { get; set; }
    }
}
