using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Project.Models;
using Project.Tree;

namespace Project.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EbenEzerController : ControllerBase
    {
        delegate string ObjectToString(object obj);
        delegate object StringToObject(string str);
        delegate object Modify(object obj, string[] strArray);
        LZWCompression LZWcom = new LZWCompression();
        LZWDecompression LZWde = new LZWDecompression();
        public static IWebHostEnvironment _environment;

        public EbenEzerController(IWebHostEnvironment env) { _environment = env; }

        [HttpPost("AgregarClave/{key}")]
        public void PostKey(int key)
        {
            TreeSingleton.Instance.Key = key;
        }

        [HttpPost("AgregarSucursal")]
        public void AddBranchOffice([FromForm]BranchOfficeRequest objBra)
        {
            BStarTree<BranchOfficeRequest>.StartTree("BranchOffice",
            new StringToObject(BranchOfficeRequest.StringToObject),
            new ObjectToString(BranchOfficeRequest.ObjectToString));
            SDES.GetKey();
            BStarTree<BranchOfficeRequest>.InsertTree(new BranchOfficeRequest { ID = BStarTree<BranchOfficeRequest>.CreateId(), Name = objBra.Name, Address = objBra.Address });
        }

        [HttpPost("AgregarProducto")]
        public void AddProduct([FromForm]ProductRequest objPro)
        {
            BStarTree<ProductRequest>.StartTree("Product",
            new StringToObject(ProductRequest.StringToObject),
            new ObjectToString(ProductRequest.ObjectToString));
            SDES.GetKey();
            BStarTree<ProductRequest>.InsertTree(new ProductRequest { ID = BStarTree<ProductRequest>.CreateId(), Name = objPro.Name, Price = objPro.Price });
        }

        //With .csv
        [HttpPost("AgregarProductos")]
        public void AddProducts([FromForm]IFormFile objcsv)
        {
            BStarTree<ProductRequest>.StartTree("Product",
            new StringToObject(ProductRequest.StringToObject),
            new ObjectToString(ProductRequest.ObjectToString));
            SDES.GetKey();
            ProductRequest.InsertCSV(objcsv.OpenReadStream());
        }

        [HttpPost("AgregarProductoSucursal")]
        public void AddBranchOfficeProduct([FromForm]BranchProductRequest objBraPro)
        {
            BStarTree<BranchProductRequest>.StartTree("BranchProduct",
            new StringToObject(BranchProductRequest.StringToObject),
            new ObjectToString(BranchProductRequest.ObjectToString));
            SDES.GetKey();
            BStarTree<BranchProductRequest>.InsertTree(new BranchProductRequest { IdBranch = objBraPro.IdBranch, IdProduct = objBraPro.IdProduct, Inventory = objBraPro.Inventory });
        }

        [HttpPost("Import")]
        public void Decompress([FromForm]IFormFile objFile)
        {
            if (!Directory.Exists(_environment.WebRootPath + "\\Project\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Project\\");
            using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Project\\" + "LZWDecompress" + ".txt");
            _fileStream.Flush();
            _fileStream.Close();
            LZWde.Decompression(_environment.WebRootPath + "\\Project\\" + objFile.FileName, _environment.WebRootPath + "\\Project\\" + "LZWDecompress" + ".txt");
        }

        [HttpPost("ActualizarSucursal")]
        public void UpdateBranchOffice([FromForm]BranchOfficeRequest objBraOff)
        {
            SDES.GetKey();
            BStarTree<BranchOfficeRequest>.StartTree("BranchOffice",
            new StringToObject(BranchOfficeRequest.StringToObject),
            new ObjectToString(BranchOfficeRequest.ObjectToString));
            BStarTree<BranchOfficeRequest>.Modify(objBraOff, new string[2] { objBraOff.Name, objBraOff.Address }, new Modify(BranchOfficeRequest.Update));
        }

        [HttpPost("ActualizarProducto")]
        public void UpdateProduct([FromForm]ProductRequest objPro)
        {
            SDES.GetKey();
            BStarTree<ProductRequest>.StartTree("Product",
            new StringToObject(ProductRequest.StringToObject),
            new ObjectToString(ProductRequest.ObjectToString));
            BStarTree<ProductRequest>.Modify(objPro, new string[2] { objPro.Name, objPro.Price.ToString() }, new Modify(ProductRequest.Update));
        }

        [HttpPost("ActualizarUbicacion")]
        public void UpdateBranchOfficeProduct([FromForm]BranchProductRequest info)
        {
            SDES.GetKey();
            BStarTree<BranchProductRequest>.StartTree("BranchProduct", new StringToObject(BranchProductRequest.StringToObject), new ObjectToString(BranchProductRequest.ObjectToString));
            BStarTree<BranchProductRequest>.Modify(info, new string[2] { info.Inventory.ToString(), null }, new Modify(BranchProductRequest.Update));
        }

        [HttpPost("Transferir")]
        public void Transfer([FromForm]TransferRequest objTra)
        {
            SDES.GetKey();
            BStarTree<BranchProductRequest>.StartTree("BranchProduct",
            new StringToObject(BranchProductRequest.StringToObject),
            new ObjectToString(BranchProductRequest.ObjectToString));
            var ArrayA = BStarTree<BranchProductRequest>.Road(new BranchProductRequest { IdBranch = objTra.id1, IdProduct = objTra.idProduct }, 1);
            var ArrayB = BStarTree<BranchProductRequest>.Road(new BranchProductRequest { IdBranch = objTra.id2, IdProduct = objTra.idProduct }, 1);

            if (ArrayA.Count != 0 && ArrayA[0].Inventory - objTra.Quantity >= 0)
            {
                if (ArrayB.Count == 0) BStarTree<BranchProductRequest>.InsertTree(new BranchProductRequest { IdBranch = objTra.id2, IdProduct = objTra.idProduct, Inventory = objTra.Quantity });
                else
                {
                    ArrayB[0].Inventory = ArrayB[0].Inventory + objTra.Quantity;
                    BStarTree<BranchProductRequest>.Modify(ArrayB[0], new string[2] { ArrayB[0].Inventory.ToString(), "" }, new Modify(BranchProductRequest.Update));
                }

                ArrayA[0].Inventory = ArrayA[0].Inventory - objTra.Quantity;
                BStarTree<BranchProductRequest>.Modify(ArrayA[0], new string[2] { ArrayA[0].Inventory.ToString(), "" }, new Modify(BranchProductRequest.Update));
            }
        }

        [HttpGet("BuscarProductos")]
        public List<ProductRequest> AllProduct()
        {
            BStarTree<ProductRequest>.StartTree("Product",
            new StringToObject(ProductRequest.StringToObject),
            new ObjectToString(ProductRequest.ObjectToString));
            SDES.GetKey();
            return BStarTree<ProductRequest>.Road(null);
        }

        [HttpGet("BuscarSucursales")]
        public List<BranchOfficeRequest> AllOffice()
        {
            BStarTree<BranchOfficeRequest>.StartTree("BranchOffice",
            new StringToObject(BranchOfficeRequest.StringToObject),
            new ObjectToString(BranchOfficeRequest.ObjectToString));
            SDES.GetKey();
            return BStarTree<BranchOfficeRequest>.Road(null);
        }

        [HttpGet("BuscarUbicaciones")]
        public List<BranchProductRequest> AllBranchProductRequest()
        {
            BStarTree<BranchProductRequest>.StartTree("BranchProduct",
            new StringToObject(BranchProductRequest.StringToObject),
            new ObjectToString(BranchProductRequest.ObjectToString));
            SDES.GetKey();
            return BStarTree<BranchProductRequest>.Road(null);
        }

        [HttpGet("BuscarProducto/{id}")]
        public List<ProductRequest> ProductShow(int id)
        {
            BStarTree<ProductRequest>.StartTree("Product",
            new StringToObject(ProductRequest.StringToObject),
            new ObjectToString(ProductRequest.ObjectToString));
            SDES.GetKey();
            return BStarTree<ProductRequest>.Road(new ProductRequest { ID = id }, 1);
        }

        [HttpGet("BuscarSucursal/{id}")]
        public List<BranchOfficeRequest> OfficeShow(int id)
        {
            BStarTree<BranchOfficeRequest>.StartTree("BranchOffice",
            new StringToObject(BranchOfficeRequest.StringToObject),
            new ObjectToString(BranchOfficeRequest.ObjectToString));
            SDES.GetKey();
            return BStarTree<BranchOfficeRequest>.Road(new BranchOfficeRequest { ID = id }, 1);
        }

        [HttpGet("BuscarUbicacion/{idBra}/{idPro}")]
        public List<BranchProductRequest> BranchProductRequestShow(int idBra, int idPro)
        {
            BStarTree<BranchProductRequest>.StartTree("BranchProduct",
            new StringToObject(BranchProductRequest.StringToObject),
            new ObjectToString(BranchProductRequest.ObjectToString));
            SDES.GetKey();
            return BStarTree<BranchProductRequest>.Road(new BranchProductRequest { IdBranch = idBra, IdProduct = idPro }, 1);
        }

        [HttpGet("Export/{name}")]
        public async Task<FileStreamResult> Compress(string name)
        {
            if (!Directory.Exists(_environment.WebRootPath + "\\Project\\")) Directory.CreateDirectory(_environment.WebRootPath + "\\Project\\");
            using var _fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Project\\" + "LZWCompress" + ".txt");
            _fileStream.Flush();
            _fileStream.Close();
            LZWcom.Compression(Path.GetFullPath(name), _environment.WebRootPath + "\\Project\\" + "LZWCompress" + ".txt");
            return await Download(_environment.WebRootPath + "\\Project\\" + "LZWCompress" + ".lzw");
        }

        async Task<FileStreamResult> Download(string path)
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, MediaTypeNames.Application.Octet, Path.GetFileName(path));
        }
    }
}