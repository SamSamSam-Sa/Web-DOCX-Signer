using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Vml.Spreadsheet;

namespace Paste_to_word
{
    class Program
    {

        static void Main(string[] args)
        {

            //string filepathFrom = @"C:\Users\stasy\Desktop\testWordDir\Sam.docx";
            string filepathTo = @"C:\Users\stasy\Desktop\testWordDir\Sam.docx";
            //SignatureCreator.CreateTable(filepathTo);
            Signature.PasteToFooter(filepathTo);
        }

    }
}