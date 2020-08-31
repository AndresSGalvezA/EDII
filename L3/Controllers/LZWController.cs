using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using L3.Models;
using System.IO;

namespace L3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LZWController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        readonly LZWCompression LZWComp = new LZWCompression();
        readonly LZWDecompression LZWDecomp = new LZWDecompression();
        
        public LZWController(IWebHostEnvironment env)
        {
            _environment = env;
        }


        [Route("/Compress/{id}/LZW")]
        [HttpPost]
        public async Task<IActionResult> UploadFileTxt([FromForm] FileUploadAPI objFile, string id)
        {
            if (objFile.Files.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\UploadLZW\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\UploadLZW\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\UploadLZW\\" + objFile.Files.FileName);
                objFile.Files.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();
                string[] FileNameSplited = objFile.Files.FileName.Split(".");
                LZWComp.Compression(_environment.WebRootPath + "\\UploadLZW\\" + objFile.Files.FileName, _environment.WebRootPath + "\\UploadLZW\\" + id + ".lzw", FileNameSplited[0]);
                LZWComp.SetCompressionsLZW(_environment.WebRootPath + "\\UploadLZW\\" + objFile.Files.FileName, _environment.WebRootPath + "\\UploadLZW\\" + id + ".lzw");

                //This code return the file.
                var memory = new MemoryStream();

                using (var stream = new FileStream(_environment.WebRootPath + "\\UploadLZW\\" + id + ".lzw", FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, id + ".lzw");
            }

            return null;
        }

        [Route("/Decompress/LZW")]
        [HttpPost]
        public async Task<IActionResult> UploadFileLZW([FromForm] FileUploadAPI objFile)
        {
            if (objFile.Files.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\UploadLZW\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\UploadLZW\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\UploadLZW\\" + objFile.Files.FileName);
                objFile.Files.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();
                LZWDecomp.Decompression(_environment.WebRootPath + "\\UploadLZW\\" + objFile.Files.FileName, _environment.WebRootPath + "\\UploadLZW\\" + LZWDecomp.GetOriginalName(_environment.WebRootPath + "\\UploadLZW\\" + objFile.Files.FileName) + ".txt");
                var memory = new MemoryStream();

                using (var stream = new FileStream(_environment.WebRootPath + "\\UploadLZW\\" + LZWDecomp.GetOriginalName(_environment.WebRootPath + "\\UploadLZW\\" + objFile.Files.FileName) + ".txt", FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, LZWDecomp.GetOriginalName(_environment.WebRootPath + "\\UploadLZW\\" + objFile.Files.FileName) + ".txt");
            }

            return null;
        }
    }
}