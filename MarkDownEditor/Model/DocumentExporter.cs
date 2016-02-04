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
                { "PDF", new PdfExporter()}
            };

        static public bool CanExport(string typeName) 
            => Exporters.Keys.Contains(typeName);

        static public void Export(string typeName, 
            string markdownType,  string sourceCodePath,string outputPath)
        {
            Exporters[typeName].Export(markdownType, sourceCodePath, outputPath);
        }
    }

    public interface IDocumentExporter
    {
         void Export(string markdownType, string sourceCodePath, string outputPath);
    }

    public class PlainHTMLExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string outputPath)
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
        public void Export(string markdownType, string sourceCodePath, string outputPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = $"\"{sourceCodePath}\" -f {markdownType} -t html --ascii -s -H theme.css -o \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }
    }

    public class RFTExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string outputPath)
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
        public void Export(string markdownType, string sourceCodePath, string outputPath)
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
        public void Export(string markdownType, string sourceCodePath, string outputPath)
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
        public void Export(string markdownType, string sourceCodePath, string outputPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = $"\"{sourceCodePath}\" -f {markdownType} -s -o --latex-engin=xelatex --template=xelatex.template \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }
    }

    public class PdfExporter : IDocumentExporter
    {
        public void Export(string markdownType, string sourceCodePath, string outputPath)
        {
            var tmpFilePath = Path.GetTempFileName()+".html";
            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = $"\"{sourceCodePath}\" -f {markdownType} -t html --ascii -s -H theme.css -o \"{tmpFilePath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            process = new Process();
            process.StartInfo.FileName = "wkhtmltopdf";
            process.StartInfo.Arguments = $"\"{tmpFilePath}\" \"{outputPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            File.Delete(tmpFilePath);
        }
    }
}
