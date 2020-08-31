using Microsoft.AspNetCore.Http;

namespace LR.Models
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