using Microsoft.AspNetCore.Http;

namespace L5.Models
{
    public class RSARequest
    {
        public IFormFile File { get; set; }
        public int P { get; set; }
        public int Q { get; set; }
    }
}