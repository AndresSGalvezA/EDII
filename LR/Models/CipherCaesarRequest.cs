using Microsoft.AspNetCore.Http;

namespace LR.Models
{
    public class CipherCaesarRequest : ICipherRequest<string>
    {
        public IFormFile File { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
    }
}