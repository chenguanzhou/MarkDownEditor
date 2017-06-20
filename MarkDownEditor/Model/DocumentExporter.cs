using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WkHtmlToXDotNet;

namespace MarkDownEditor.Model
{
    public class DocumentExporter
    {
        static private Dictionary<string, IDocumentExporter> Exporters =
            new Dictionary<string, IDocumentExporter>()
            {
                { "Plain Html", new PlainHTMLExporter()},
                { "Html", new HTMLExporter()},
                { "Html Local Mathjax", new HTMLWithLocalMathJaxExporter()},
                { "RTF", new RFTExporter()},
                { "Docx", new DocxExporter()},
                { "Epub", new EpubExporter()},
                { "Latex", new LatexExporter()},
                { "PDF", new PdfExporter()},
                { "Image", new ImageExporter()}
            };

        static public bool CanExport(string typeName) 
            => Exporters.Keys.Contains(typeName);

        static public void Export(string typeName, string markdownType, 
            string cssFile, string sourceCodePath,string outputPath)
        {
            Exporters[typeName].Export(markdownType, sourceCodePath, cssFile, outputPath);
        }
    }

    public interface IDocumentExporter
    {
         void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath);
    }

    public class PlainHTMLExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = $"\"{sourceCodePath}\" -f {markdownType} -t html --ascii -s -o \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }
    }

    public class HTMLExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath)
        {
            var tmpFile = Path.GetTempFileName();
            if (cssFile!=null)
            {
                StreamWriter sw = new StreamWriter(tmpFile);
                sw.WriteLine("<style type=\"text/css\">");

                if (File.Exists(cssFile))
                {
                    StreamReader sr = new StreamReader(cssFile);
                    var cssContent = sr.ReadToEnd();
                    sr.Close();
                    sw.WriteLine(cssContent);
                }
                
                sw.WriteLine("</style>");
                sw.Close();
            }

            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            string mathjax = Properties.Settings.Default.ShowMathJax ? "--mathjax" : "";
            process.StartInfo.Arguments = 
                cssFile == null 
                ? $"\"{sourceCodePath}\" -f {markdownType} -t html {mathjax} --ascii -s -o \"{outputPath}\""
                : $"\"{sourceCodePath}\" -f {markdownType} -t html {mathjax} --ascii -s -H {tmpFile} -o \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            File.Delete(tmpFile);
        }
    }

    public class HTMLWithLocalMathJaxExporter : IDocumentExporter
    {
        string mathjaxJsFilePath = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MathJax", "MathJax.js")).AbsoluteUri + "?config=TEX_CHTML";
        public void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath)
        {
            var tmpFile = Path.GetTempFileName();
            if (cssFile != null)
            {
                StreamWriter sw = new StreamWriter(tmpFile);
                sw.WriteLine("<style type=\"text/css\">");

                if (File.Exists(cssFile))
                {
                    StreamReader sr = new StreamReader(cssFile);
                    var cssContent = sr.ReadToEnd();
                    sr.Close();
                    sw.WriteLine(cssContent);
                }

                sw.WriteLine("</style>");
                sw.Close();
            }

            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            string mathjax = Properties.Settings.Default.ShowMathJax ? $"--mathjax={mathjaxJsFilePath}" : "";
            process.StartInfo.Arguments =
                cssFile == null
                ? $"\"{sourceCodePath}\" -f {markdownType} -t html {mathjax} --ascii -s -o \"{outputPath}\""
                : $"\"{sourceCodePath}\" -f {markdownType} -t html {mathjax} --ascii -s -H {tmpFile} -o \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            File.Delete(tmpFile);
        }
    }

    public class RFTExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = $"\"{sourceCodePath}\" -f {markdownType} -t rtf -s -o \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }
    }

    public class DocxExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = $"\"{sourceCodePath}\" -f {markdownType} -t docx -o \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }
    }

    public class EpubExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = $"\"{sourceCodePath}\" -f {markdownType} -t epub -o \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }
    }

    public class LatexExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = $"\"{sourceCodePath}\" -f {markdownType} -s --latex-engine=xelatex --template=xelatex.template -o \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }
    }

    public class PdfExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath)
        {
            
            var tmpHtmlPath = Path.GetDirectoryName(sourceCodePath) + "\\~" + Path.GetRandomFileName() + ".html"; ;

            DocumentExporter.Export("Html", markdownType, cssFile, sourceCodePath, tmpHtmlPath);

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            //File.WriteAllBytes(outputPath, HtmlToXConverter.ConvertToPdf(File.ReadAllText(tmpFilePath)));
            Process process = new Process();
            process.StartInfo.FileName = "WkHtmlToPdfWrapper";
            process.StartInfo.Arguments = $"\"{tmpHtmlPath}\" \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new Exception(Properties.Resources.FailedToExport + "\n" + "wkhtmltopdf error" + process.ExitCode);

            File.Delete(tmpHtmlPath);
        }
    }

    public class ImageExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath)
        {
            var tmpFilePath = Path.GetTempFileName() + ".html";

            DocumentExporter.Export("Html", markdownType, cssFile, sourceCodePath, tmpFilePath);

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            Process process = new Process();
            process.StartInfo.FileName = "WkHtmlToImageWrapper";
            process.StartInfo.Arguments = $"\"{tmpFilePath}\" \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new Exception(Properties.Resources.FailedToExport + "\n" + "wkhtmltoimage error" + process.ExitCode);

            //var html = File.ReadAllText(tmpFilePath);
            //var extension = Path.GetExtension(outputPath).Remove(0, 1).ToLower();
            //var image = HtmlToXConverter.ConvertToImage(html, extension, 600, 0);
            //File.WriteAllBytes(outputPath, image);

            File.Delete(tmpFilePath);
        }
    }
}


