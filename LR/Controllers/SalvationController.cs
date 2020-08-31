using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace LR.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SalvationController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Movie", "Compressions", "Compress/{id}/Huffman", "Decompress/Huffman", "Compress/{id}/LZW",
                "Decompress/LZW", "cipher", "cipher/caesar", "cipher/zigzag", "cipher/route", "decipher/caesar", "decipher/zigzag",
                "decipher/route", "cipher2/GetPublicKey", "cipher2/Caesar2/RSA", "decipher/Caesar2/RSA", "Cipher2/Caesar2/DiffieHellman", 
                "Decipher/Caesar2/DiffieHellman" };
        }
    }
}