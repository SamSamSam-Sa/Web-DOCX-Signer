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
using System.Text;

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
        public async Task<string> SignDocuments([FromForm]IFormFileCollection uploads, [FromForm] string firstName, [FromForm] string secondName)
        {
            //HttpResponseMessage result = Request.Create(HttpStatusCode.OK);
            var filename = "SignedFiles.zip";
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

                // путь к папке Files
                var path = "/Files/" + filename;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    archiveMemoryStream.Seek(0, SeekOrigin.Begin);
                    await archiveMemoryStream.CopyToAsync(fileStream);
                }

                //using (var fileStream = new FileStream(@"C:\Users\stasy\Desktop\test.zip", FileMode.Create))
                //{
                //    archiveMemoryStream.Seek(0, SeekOrigin.Begin);
                //    archiveMemoryStream.CopyTo(fileStream);
                //}

                //var content = new StringContent(path, Encoding.Unicode);
                //result.Content = content;
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

            return filename;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetSignedDocuments(string path)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            using (var archiveFileStream = new FileStream(_appEnvironment.WebRootPath + "/Files/" + path, FileMode.Open))
            using(var archiveMemoryStream = new MemoryStream())
            {
                await archiveFileStream.CopyToAsync(archiveMemoryStream);
                var content = new ByteArrayContent(archiveMemoryStream.ToArray());
                result.Content = content;
                result.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "SignedFiles.zip"
                    };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/zip");
            }
            return result;
        }
    }
}
