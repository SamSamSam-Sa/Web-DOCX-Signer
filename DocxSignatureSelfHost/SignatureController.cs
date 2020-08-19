using DocxSignature;
using System;
using System.IO;
using System.Web.Http;

namespace DocxSignatureSelfHost
{
    [RoutePrefix("api/signature")]
    public class SignatureController : ApiController
    {
        private ISignature _signature;

        public SignatureController()
        {
            _signature = new Signature();
        }

        [HttpPost]
        public string PostSignDocuments([FromBody] string base64String, [FromBody] string firstName, [FromBody] string secondName)
        {
            string signedFileBase64 = "";

            var bytes = Convert.FromBase64String(base64String);

            using (var fileMemoryStream = new MemoryStream(bytes))
            {
                fileMemoryStream.Seek(0, SeekOrigin.Begin);
                _signature.SignDocument(fileMemoryStream, firstName, secondName);

                fileMemoryStream.Seek(0, SeekOrigin.Begin);
                signedFileBase64 = ConvertStreamToBase64(fileMemoryStream);
            }

            return signedFileBase64;
        }

        private static string ConvertStreamToBase64(MemoryStream stream)
        {
            var bytes = stream.ToArray();
            String fileInBase64 = Convert.ToBase64String(bytes);

            return fileInBase64;
        }

    }
}
