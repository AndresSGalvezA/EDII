using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using L5.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace L5.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CipherController : ControllerBase
    {
        RSA rsa = new RSA();
        public static IWebHostEnvironment _environment;
        public CipherController(IWebHostEnvironment _env)
        {
            _environment = _env;
        }
        [Route("GetPublicKey")]
        [HttpGet]

        public int Get()
        {
            return -1; //Temporal.
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
                string FilePublic = "PublicKey.txt";
                string FilePrivate = "PrivateKey.txt";

                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher2\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher2\\");
                using var _fileStreamPublic = System.IO.File.Create(_environment.WebRootPath + "\\Cipher2\\" + FilePublic);
                _fileStreamPublic.Flush();
                _fileStreamPublic.Close();

                if (!Directory.Exists(_environment.WebRootPath + "\\Cipher2\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Cipher2\\");
                using var _fileStreamPrivate = System.IO.File.Create(_environment.WebRootPath + "\\Cipher2\\" + FilePrivate);
                _fileStreamPrivate.Flush();
                _fileStreamPrivate.Close();

                rsa.GenerateKey(obj.P, obj.Q, _environment.WebRootPath + "\\Cipher2\\" + obj.File.FileName, _environment.WebRootPath + "\\Cipher2\\" + FilePublic, _environment.WebRootPath + "\\Cipher2\\" + FilePrivate);

                return DownloadZIP();


            }

            return null;


        }

        [Route("Caesar2/DiffieHellman")]
        [HttpPost]
        public void CipherDiffieHellman([FromForm] DiffieHellmanRequest obj)
        {

        }
        public async Task<IActionResult> GetFile(string folder)
        {
            var memory = new MemoryStream();

            using (var stream = new FileStream(folder, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            Directory.Delete(_environment.WebRootPath + "\\Cipher2\\" , true);
            return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(folder)); ;
        }
        async Task<IActionResult> DownloadZIP()
        {
            var archivePath = Path.Combine(Environment.CurrentDirectory, "archive.zip");
            var Cipher2Path = Path.Combine(_environment.WebRootPath, "Cipher2");
            if (System.IO.File.Exists(archivePath))
            {
                System.IO.File.Delete(archivePath);
            }
            ZipFile.CreateFromDirectory(Cipher2Path,archivePath);
            return await GetFile(archivePath);
        }


    }
}