using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LR.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompressionsController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        readonly HuffmanCompression HuffComp = new HuffmanCompression();
        readonly HuffmanDecompression HuffDecomp = new HuffmanDecompression();
        readonly LZWCompression LZWComp = new LZWCompression();
        readonly LZWDecompression LZWDecomp = new LZWDecompression();

        public CompressionsController(IWebHostEnvironment env)
        {
            _environment = env;
        }

        [Route("/Compressions")]
        [HttpGet]
        public Stack<string> Get()
        {
            return HuffComp.GetCompressions();
        }

        [Route("/Compress/{id}/Huffman")]
        [HttpPost]
        public async Task<IActionResult> UploadFileCompress([FromForm] FileUploadAPI objFile, string id)
        {
            if (objFile.Files.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\UploadHuffman\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\UploadHuffman\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\UploadHuffman\\" + objFile.Files.FileName);
                objFile.Files.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();
                string[] FileNameSplited = objFile.Files.FileName.Split(".");
                HuffComp.Compression(_environment.WebRootPath + "\\UploadHuffman\\" + objFile.Files.FileName, _environment.WebRootPath + "\\UploadHuffman\\" + id + ".huff", FileNameSplited[0]);
                HuffComp.SetCompressions(_environment.WebRootPath + "\\UploadHuffman\\" + objFile.Files.FileName, _environment.WebRootPath + "\\UploadHuffman\\" + id + ".huff");

                //This code return the file.
                var memory = new MemoryStream();

                using (var stream = new FileStream(_environment.WebRootPath + "\\UploadHuffman\\" + id + ".huff", FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, id + ".huff");
            }

            return null;
        }

        [Route("/Decompress/Huffman")]
        [HttpPost]
        public async Task<IActionResult> UploadFileDecompress([FromForm] FileUploadAPI objFile)
        {
            if (objFile.Files.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\UploadHuffman\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\UploadHuffman\\");
                using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\UploadHuffman\\" + objFile.Files.FileName);
                objFile.Files.CopyTo(_fileStream);
                _fileStream.Flush();
                _fileStream.Close();
                HuffDecomp.Decompression(_environment.WebRootPath + "\\UploadHuffman\\" + objFile.Files.FileName, _environment.WebRootPath + "\\UploadHuffman\\" + HuffDecomp.GetOriginalName(_environment.WebRootPath + "\\UploadHuffman\\" + objFile.Files.FileName) + ".txt");

                var memory = new MemoryStream();

                using (var stream = new FileStream(_environment.WebRootPath + "\\UploadHuffman\\" + HuffDecomp.GetOriginalName(_environment.WebRootPath + "\\UploadHuffman\\" + objFile.Files.FileName) + ".txt", FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, HuffDecomp.GetOriginalName(_environment.WebRootPath + "\\UploadHuffman\\" + objFile.Files.FileName) + ".txt");
            }

            return null;
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