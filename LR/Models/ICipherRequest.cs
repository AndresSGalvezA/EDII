using Microsoft.AspNetCore.Http;

namespace LR.Models
{
    public interface ICipherRequest<T>
    {
        IFormFile File { get; set; }
        T Key { get; set; }
        string Name { get; set; }

    }
}