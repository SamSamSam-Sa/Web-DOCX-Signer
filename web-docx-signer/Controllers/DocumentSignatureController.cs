using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocxSignature;

namespace web_docx_signer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentSignatureController : ControllerBase
    {
        IWebHostEnvironment _appEnvironment;
        private ISignature _signature;

        public DocumentSignatureController(IWebHostEnvironment appEnvironment, ISignature signature)
        {
            _appEnvironment = appEnvironment;
            _signature = signature;
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFileCollection uploads)
        {
            foreach (var uploadedFile in uploads)
            {
                // путь к папке Files
                string path = "/Files/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                    fileStream.Seek(0, SeekOrigin.Begin);
                    _signature.SignDocument(fileStream, "ergerger", "ergerger");
                }

            }

            return Ok();
        }
    }
}
