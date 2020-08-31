using Microsoft.AspNetCore.Http;

namespace L4.Models
{
    public interface ICipherRequest<T>
    {
        IFormFile File { get; set; }
        T Key { get; set;  }
        string Name { get; set; }
        
    }
}