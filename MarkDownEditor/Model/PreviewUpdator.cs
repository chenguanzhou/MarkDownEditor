using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkDownEditor.ViewModel;
using System.IO;
using System.Timers;

namespace MarkDownEditor.Model
{
    public class PreviewUpdator
    {
        private Timer timer = new Timer(200);

        MainViewModel model;
        string content;
        string sourcePath;
        string previewPath;
        string markdownType;
        string cssFile;

        public PreviewUpdator()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StreamWriter sw = new StreamWriter(sourcePath);
            sw.Write(content);
            sw.Close();

            DocumentExporter.Export("Html Local Mathjax",
                markdownType,
                cssFile,
                sourcePath,
                previewPath);

            model.ShouldReload = !model.ShouldReload;
            timer.Stop();
        }

        public void Update( MainViewModel model, string content, string sourcePath, string previewPath, string markdownType, string cssFile )
        {
            timer.Stop();
            this.model = model;
            this.content = content;
            this.sourcePath = sourcePath;
            this.previewPath = previewPath;
            this.cssFile = cssFile;
            this.markdownType = markdownType;
            timer.Start();
        }
    }
}
