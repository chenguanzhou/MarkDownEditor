using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WkHtmlToXDotNet;

namespace WkHtmlToPdfWrapper
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                string inputFilePath = args[0];
                string outputFilePath = args[1];
                File.WriteAllBytes(outputFilePath, HtmlToXConverter.ConvertToPdf(File.ReadAllText(inputFilePath)));
                return 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
