using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using LR.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace LR.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Cipher2Controller : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        Caesar caesar = new Caesar();
        RSA rsa = new RSA();
        
        public Cipher2Controller(IWebHostEnvironment _env)
        {
            _environment = _env;
        }

        [Route("GetPublicKey")]
        [HttpGet]
        public Stack<string> Get1()
        {
            return rsa.GetKeys();
        }

        [Route("Caesar2/RSA")]
        [HttpPost]
        public Task<IActionResult> CipherRSA([FromForm] RSARequest obj)
        {
            if (obj.File.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher2\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher2\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Cipher2\\" + obj.File.FileName);
                obj.File.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();
                string FilePrivate = "PrivateKey.txt";

                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher2\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher2\\");
                using var _fileStreamPrivate = System.IO.File.Create(_environment.WebRootPath + "\\Cipher2\\" + FilePrivate);
                _fileStreamPrivate.Flush();
                _fileStreamPrivate.Close();
                caesar.Cipher(_environment.WebRootPath + "\\Cipher2\\" + obj.File.FileName, obj.Key, _environment.WebRootPath + "\\Cipher2\\" + obj.Name + ".txt");
                rsa.GeneratePrivateKey(obj.P, obj.Q, obj.Key, _environment.WebRootPath + "\\Cipher2\\" + FilePrivate);

                return DownloadZIP();
            }

            return null;
        }

        [Route("/decipher/Caesar2/RSA")]
        [HttpPost]
        public Task<IActionResult> DecipherRSA([FromForm] RSARequest obj)
        {
            if (obj.File.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher2\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher2\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Cipher2\\" + obj.File.FileName);
                obj.File.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();
                string FilePublic = "PublicKey.txt";

                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher2\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher2\\");
                using var _fileStreamPublic = System.IO.File.Create(_environment.WebRootPath + "\\Cipher2\\" + FilePublic);
                _fileStreamPublic.Flush();
                _fileStreamPublic.Close();
                
                caesar.Decipher(_environment.WebRootPath + "\\Cipher2\\" + obj.File.FileName, rsa.GeneratePublicKey(obj.P, obj.Q, obj.Key, _environment.WebRootPath + "\\Cipher2\\" + FilePublic), _environment.WebRootPath + "\\Cipher2\\" + obj.Name + ".txt");
                return DownloadZIP();
            }

            return null;
        }

        [Route("Caesar2/DiffieHellman")]
        [HttpPost]
        public Task<IActionResult> CipherDiffieHellman([FromForm] DiffieHellmanRequest obj)
        {
            if (obj.File.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher2\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher2\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Cipher2\\" + obj.File.FileName);
                obj.File.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();
                string Keya = "PrivateKey.txt";
                string PublicA = "MainPublicKey.txt";

                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher2\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher2\\");
                using var _fileStreamPrivate = System.IO.File.Create(_environment.WebRootPath + "\\Cipher2\\" + Keya);
                _fileStreamPrivate.Flush();
                _fileStreamPrivate.Close();

                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher2\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher2\\");
                using var _fileStreamPrivate1 = System.IO.File.Create(_environment.WebRootPath + "\\Cipher2\\" + PublicA);
                _fileStreamPrivate1.Flush();
                _fileStreamPrivate1.Close();

                DiffieHellman diffie = new DiffieHellman(obj.PublicKey);
                diffie.FilePath(_environment.WebRootPath + "\\Cipher2\\" + Keya, _environment.WebRootPath + "\\Cipher2\\" + PublicA);
                caesar.Cipher(_environment.WebRootPath + "\\Cipher2\\" + obj.File.FileName, diffie.GetCommonKey(), _environment.WebRootPath + "\\Cipher2\\" + obj.Name + ".txt");

                return DownloadZIP();
            }

            return null;
        }

        [Route("/decipher/Caesar2/DiffieHellman")]
        [HttpPost]
        public Task<IActionResult> DecipherDiffieHellman([FromForm] DiffieHellmanRequest obj)
        {
            if (obj.File.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher2\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher2\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Cipher2\\" + obj.File.FileName);
                obj.File.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();

                DiffieHellman diffie = new DiffieHellman(obj.PublicKey);
                caesar.Decipher(_environment.WebRootPath + "\\Cipher2\\" + obj.File.FileName, diffie.GetCommonKey(), _environment.WebRootPath + "\\Cipher2\\" + obj.Name + ".txt");

                return DownloadZIP();
            }

            return null;
        }

        public async Task<IActionResult> GetFile(string folder)
        {
            var memory = new MemoryStream();

            using (var stream = new FileStream(folder, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            Directory.Delete(_environment.WebRootPath + "\\Cipher2\\", true);
            return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(folder)); ;
        }

        async Task<IActionResult> DownloadZIP()
        {
            var archivePath = Path.Combine(Environment.CurrentDirectory, "archive.zip");
            var Cipher2Path = Path.Combine(_environment.WebRootPath, "Cipher2");
            if (System.IO.File.Exists(archivePath)) System.IO.File.Delete(archivePath);
            ZipFile.CreateFromDirectory(Cipher2Path, archivePath);
            return await GetFile(archivePath);
        }
    }
}