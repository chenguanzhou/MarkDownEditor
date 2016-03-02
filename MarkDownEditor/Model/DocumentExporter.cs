using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkDownEditor.Model
{
    public class DocumentExporter
    {
        static private Dictionary<string, IDocumentExporter> Exporters =
            new Dictionary<string, IDocumentExporter>()
            {
                { "Plain Html", new PlainHTMLExporter()},
                { "Html", new HTMLExporter()},
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
                StreamReader sr = new StreamReader($"css/{cssFile}");
                var cssContent = sr.ReadToEnd();
                sr.Close();
                StreamWriter sw = new StreamWriter(tmpFile);
                sw.WriteLine("<style type=\"text/css\">");
                sw.WriteLine(cssContent);
                sw.WriteLine("</style>");
                sw.Close();
            }

            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = 
                cssFile == null 
                ? $"\"{sourceCodePath}\" -f {markdownType} -t html --ascii -s -o \"{outputPath}\""
                : $"\"{sourceCodePath}\" -f {markdownType} -t html --ascii -s -H {tmpFile} -o \"{outputPath}\"";
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
            var tmpFilePath = Path.GetTempFileName()+".html";

            DocumentExporter.Export("Html", markdownType, cssFile, sourceCodePath, tmpFilePath);

            Process process = new Process();
            process.StartInfo.FileName = "wkhtmltopdf";
            process.StartInfo.Arguments = $"\"{tmpFilePath}\" \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new Exception(Properties.Resources.FailedToExport + "\n" + "wkhtmltopdf error" + process.ExitCode);

            File.Delete(tmpFilePath);
        }
    }

    public class ImageExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string cssFile, string outputPath)
        {
            var tmpFilePath = Path.GetTempFileName() + ".html";

            DocumentExporter.Export("Html", markdownType, cssFile, sourceCodePath, tmpFilePath);

            Process process = new Process();
            process.StartInfo.FileName = "wkhtmltoimage";
            process.StartInfo.Arguments = $"--width 600 \"{tmpFilePath}\" \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new Exception(Properties.Resources.FailedToExport + "\n" + "wkhtmltoimage error" + process.ExitCode);

            File.Delete(tmpFilePath);
        }
    }
}


