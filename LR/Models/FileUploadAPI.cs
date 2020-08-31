using Microsoft.AspNetCore.Http;

namespace LR.Models
{
    public class FileUploadAPI
    {
        public IFormFile Files { get; set; }
    }
}