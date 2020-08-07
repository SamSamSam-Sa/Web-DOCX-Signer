
using DocxSignature;

namespace Paste_to_word
{
    class Program
    {

        static void Main(string[] args)
        {

            string documentPath = args[0];

            var signature = new Signature();
            signature.SignDocument(documentPath, "FirstName", "SecondName");
        }

    }
}