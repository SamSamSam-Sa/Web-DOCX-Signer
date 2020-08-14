using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocxSignature;
using System.IO.Compression;
using System;
using web_docx_signer.Services;
using System.Runtime.InteropServices.ComTypes;

namespace web_docx_signer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentSignatureController : ControllerBase
    {
        IWebHostEnvironment _appEnvironment;
        private ISignature _signature;
        private ITempDataService _tempDataService;

        public DocumentSignatureController(IWebHostEnvironment appEnvironment, ISignature signature, ITempDataService tempDataService)
        {
            _appEnvironment = appEnvironment;
            _signature = signature;
            _tempDataService = tempDataService;
        }

        [HttpPost]
        public async Task<string> SignDocuments([FromForm] IFormFileCollection uploads, [FromForm] string firstName, [FromForm] string secondName)
        {
            var fileGuid = Guid.NewGuid();
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
                _tempDataService.Add(fileGuid, archiveMemoryStream.ToArray());
            }

            return fileGuid.ToString();
        }

        [HttpGet]
        public ActionResult Download(string fileGuid)
        {
            var guid = new Guid(fileGuid);
            if (_tempDataService.IsExist(guid))
            {
                return File(_tempDataService.Get(guid), "application/zip");
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}
