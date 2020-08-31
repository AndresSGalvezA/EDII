using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using LR.Models;

namespace LR.Controllers
{
    [Route("controller")]
    [ApiController]
    public class CipherController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        public static ZigZag ZCipher = new ZigZag();
        public static Caesar CaesarCipher = new Caesar();
        public static Route route = new Route();

        public CipherController(IWebHostEnvironment _env)
        {
            _environment = _env;
        }

        [HttpGet]
        public string Get()
        {
            return "Transposition cipher.";
        }

        [Route("/cipher/caesar")]
        [HttpPost]
        public Task<IActionResult> CipherCaesar([FromForm] CipherCaesarRequest CipherCaesarRequest)
        {
            if (CipherCaesarRequest.File.Length > 0)
            {
                string name = CipherCaesarRequest.File.FileName;
                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Cipher\\" + CipherCaesarRequest.File.FileName);
                CipherCaesarRequest.File.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();

                CaesarCipher.Cipher( _environment.WebRootPath + "\\Cipher\\" + CipherCaesarRequest.File.FileName, CipherCaesarRequest.Key, _environment.WebRootPath + "\\Cipher\\" + CipherCaesarRequest.Name + ".txt");
                return GetFile(Convert.ToString(CipherCaesarRequest.Name), "\\Cipher\\");
            }

            return null;
        }

        [Route("/cipher/zigzag")]
        [HttpPost]
        public Task<IActionResult> CipherZigZag([FromForm] CipherZigZagRequest CipherZigZagRequest)
        {
            if (CipherZigZagRequest.File.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Cipher\\" + CipherZigZagRequest.File.FileName);
                CipherZigZagRequest.File.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();

                ZCipher.Cipher(_environment.WebRootPath + "\\Cipher\\" + CipherZigZagRequest.File.FileName, _environment.WebRootPath + "\\Cipher\\" + CipherZigZagRequest.Name + ".txt", CipherZigZagRequest.Key);
                return GetFile(Convert.ToString(CipherZigZagRequest.Name), "\\Cipher\\");
            }

            return null;
        }

        [Route("/cipher/route")]
        [HttpPost]
        public Task<IActionResult> CipherRoute([FromForm] CipherRouteRequest CipherRouteRequest)
        {
            if (CipherRouteRequest.File.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Cipher\\" + CipherRouteRequest.File.FileName);
                CipherRouteRequest.File.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();
                route.Cipher(CipherRouteRequest.Direction, _environment.WebRootPath + "\\Cipher\\" + CipherRouteRequest.File.FileName, _environment.WebRootPath + "\\Cipher\\" + CipherRouteRequest.Name + ".txt", CipherRouteRequest.Key, CipherRouteRequest.KeyColumns);
                return GetFile(Convert.ToString(CipherRouteRequest.Name), "\\Cipher\\");
            }

            return null;
        }

        [Route("/decipher/caesar")]
        [HttpPost]
        public Task<IActionResult> DecipherCaesar([FromForm] CipherCaesarRequest DecipherCaesarRequest)
        {
            if (DecipherCaesarRequest.File.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Decipher\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Decipher\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Decipher\\" + DecipherCaesarRequest.File.FileName);
                DecipherCaesarRequest.File.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();

                CaesarCipher.Decipher( _environment.WebRootPath + "\\Decipher\\" + DecipherCaesarRequest.File.FileName, DecipherCaesarRequest.Key, _environment.WebRootPath + "\\Decipher\\" + DecipherCaesarRequest.Name + ".txt");
                return GetFile(Convert.ToString(DecipherCaesarRequest.Name), "\\Decipher\\");
            }

            return null;
        }

        [Route("/decipher/zigzag")]
        [HttpPost]
        public Task<IActionResult> DecipherZigZag([FromForm] CipherZigZagRequest DecipherZigZagRequest)
        {
            if (DecipherZigZagRequest.File.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Decipher\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Decipher\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Decipher\\" + DecipherZigZagRequest.File.FileName);
                DecipherZigZagRequest.File.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();

                ZCipher.Decipher(_environment.WebRootPath + "\\Decipher\\" + DecipherZigZagRequest.File.FileName, _environment.WebRootPath + "\\Decipher\\" + DecipherZigZagRequest.Name + ".txt", DecipherZigZagRequest.Key);
                return GetFile(Convert.ToString(DecipherZigZagRequest.Name), "\\Decipher\\");
            }

            return null;
        }

        [Route("/decipher/route")]
        [HttpPost]
        public Task<IActionResult> DecipherRoute([FromForm] CipherRouteRequest DecipherRouteRequest)
        {
            if (DecipherRouteRequest.File.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Decipher\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Decipher\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Decipher\\" + DecipherRouteRequest.File.FileName);
                DecipherRouteRequest.File.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();
                route.Decipher(DecipherRouteRequest.Direction, _environment.WebRootPath + "\\Decipher\\" + DecipherRouteRequest.File.FileName, _environment.WebRootPath + "\\Decipher\\" + DecipherRouteRequest.Name + ".txt", DecipherRouteRequest.Key, DecipherRouteRequest.KeyColumns);
                return GetFile(Convert.ToString(DecipherRouteRequest.Name), "\\Decipher\\");
            }

            return null;
        }

        public async Task<IActionResult> GetFile(string Name, string folder)
        {
            var memory = new MemoryStream();

            using (var stream = new FileStream(_environment.WebRootPath + folder + Name + ".txt", FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, Name + ".txt");
        }
    }
}