using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocxSignature;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

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
        public HttpResponseMessage AddFile([FromForm]IFormFileCollection uploads, [FromForm] string firstName, [FromForm] string secondName)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            using (var archiveMemoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveMemoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var uploadedFile in uploads)
                    {
                        using (var fileMemoryStream = new MemoryStream())
                        {
                            uploadedFile.CopyTo(fileMemoryStream);
                            fileMemoryStream.Seek(0, SeekOrigin.Begin);
                            _signature.SignDocument(fileMemoryStream, firstName, secondName);
                            fileMemoryStream.Seek(0, SeekOrigin.Begin);

                            var archiveFile = archive.CreateEntry(uploadedFile.FileName);
                            using (var entryStream = archiveFile.Open())
                            {
                                using (var streamWriter = new StreamWriter(entryStream))
                                {
                                    fileMemoryStream.CopyTo(entryStream);
                                }
                            }
                        }
                    }
                }

                archiveMemoryStream.Seek(0, SeekOrigin.Begin);
                var content = new StreamContent(archiveMemoryStream);
                result.Content = content;
                result.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "zip.zip"
                    };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");

            }

            //using(var stream  = new MemoryStream())
            //{
            //    uploads[0].CopyTo(stream);
            //    var content = new StreamContent(stream);

            //    result.Content = content;                
            //}

            //result.Content.Headers.ContentDisposition =
            //           new ContentDispositionHeaderValue("attachment")
            //           {
            //               FileName = "zip.zip"
            //           };
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip") ;

            return result;
        }
    }
}
