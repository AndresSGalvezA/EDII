using Microsoft.AspNetCore.Http;

namespace L5.Models
{
    public class DiffieHellmanRequest
    {
        public IFormFile FormFile { get; set; }
        public int PublicKey { get; set; }
    }
}