using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace L4.Models
{
    public class CipherCaesarRequest : ICipherRequest<string>
    {
        public IFormFile File { get ; set ; }
        public string Key { get; set ; }
        public string Name { get ; set; }
    }
}
