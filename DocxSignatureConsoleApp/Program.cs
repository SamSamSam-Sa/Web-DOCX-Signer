
using DocxSignature;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Paste_to_word
{
    class Program
    {

        static void Main(string[] args)
        {
            string documentPath = args[0];
            //signature.SignDocument(documentPath, "FirstName", "SecondName");
            var signatureService = new Signature();
            CreateSignedArchive(signatureService, documentPath, "FirstName", "SecondName");

        }
        public static void CreateSignedArchive(ISignature signatureService, string documentPath, string firstName, string secondName)
        {
            //var result = new HttpResponseMessage(HttpStatusCode.OK);
            using (var archiveMemoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveMemoryStream, ZipArchiveMode.Create, true))
                {
                    using (FileStream source = File.Open(documentPath, FileMode.Open))
                    using (var fileMemoryStream = new MemoryStream())
                    {
                        source.CopyTo(fileMemoryStream);
                        fileMemoryStream.Seek(0, SeekOrigin.Begin);
                        signatureService.SignDocument(fileMemoryStream, firstName, secondName);
                        fileMemoryStream.Seek(0, SeekOrigin.Begin);

                        var archiveFile = archive.CreateEntry(source.Name);
                        using (var entryStream = archiveFile.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream))
                            {
                                fileMemoryStream.CopyTo(entryStream);
                            }
                        }
                    }

                }

                using (var fileStream = new FileStream(@"C:\Users\stasy\Desktop\test.zip", FileMode.Create))
                {
                    archiveMemoryStream.Seek(0, SeekOrigin.Begin);
                    archiveMemoryStream.CopyTo(fileStream);
                }

                //result.Content = new ByteArrayContent(archiveMemoryStream.ToArray());
                //result.Content.Headers.ContentDisposition =
                //    new ContentDispositionHeaderValue("attachment")
                //    {
                //        FileName = "zip.zip"
                //    };
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            }
            //return result;
        }


    }
}