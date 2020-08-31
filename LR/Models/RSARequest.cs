using Microsoft.AspNetCore.Http;

namespace LR.Models
{
    public class RSARequest
    {
        public IFormFile File { get; set; }
        public string Name { get; set; }
        public int Key { get; set; }
        public int P { get; set; }
        public int Q { get; set; }
    }
}