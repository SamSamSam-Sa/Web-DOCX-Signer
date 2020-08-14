using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocxSignature;
using System.IO.Compression;
using System;

namespace web_docx_signer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentSignatureController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        private ISignature _signature;

        public DocumentSignatureController(IWebHostEnvironment appEnvironment, ISignature signature)
        {
            _appEnvironment = appEnvironment;
            _signature = signature;
        }

        [HttpPost]
        public async Task<string> SignDocuments([FromForm] IFormFileCollection uploads, [FromForm] string firstName, [FromForm] string secondName)
        {
            var filename = Guid.NewGuid().ToString();
            using (var archiveMemoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveMemoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var uploadedFile in uploads)
                    {
                        using (var fileMemoryStream = new MemoryStream())
                        {
                            await uploadedFile.CopyToAsync(fileMemoryStream);
                            fileMemoryStream.Seek(0, SeekOrigin.Begin);
                            _signature.SignDocument(fileMemoryStream, firstName, secondName);
                            fileMemoryStream.Seek(0, SeekOrigin.Begin);

                            var archiveFile = archive.CreateEntry(uploadedFile.FileName);
                            using (var entryStream = archiveFile.Open())
                            {
                                using (var streamWriter = new StreamWriter(entryStream))
                                {
                                    await fileMemoryStream.CopyToAsync(entryStream);
                                }
                            }
                        }
                    }
                }

                archiveMemoryStream.Seek(0, SeekOrigin.Begin);
                var temp = Convert.ToBase64String(archiveMemoryStream.ToArray());
                TempData[filename] = temp;
            }

            return filename;
        }

        [HttpGet]
        public virtual ActionResult Download(string fileGuid)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = Convert.FromBase64String((string)TempData[fileGuid]);
                return File(data, "application/zip", "SignedFiles.zip");
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}
