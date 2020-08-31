using Microsoft.AspNetCore.Http;

namespace LR.Models
{
    public class DiffieHellmanRequest
    {
        public IFormFile File { get; set; }
        public int PublicKey { get; set; }
        public string Name { get; set; }
    }
}