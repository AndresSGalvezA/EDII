using Microsoft.AspNetCore.Http;

namespace LR.Models
{
    public class CipherZigZagRequest : ICipherRequest<int>
    {
        public IFormFile File { get; set; }
        public int Key { get; set; }
        public string Name { get; set; }

    }
}