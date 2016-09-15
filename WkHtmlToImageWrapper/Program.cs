using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WkHtmlToXDotNet;

namespace WkHtmlToImageWrapper
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                string inputFilePath = args[0];
                string outputFilePath = args[1];

                var html = File.ReadAllText(inputFilePath);
                var extension = Path.GetExtension(outputFilePath).Remove(0, 1).ToLower();
                var image = HtmlToXConverter.ConvertToImage(html, extension, 600, 0);
                File.WriteAllBytes(outputFilePath, image);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
