using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MarkDownEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var languageName = MarkDownEditor.Properties.Settings.Default.Language;
            MarkDownEditor.Properties.Resources.Culture = new CultureInfo(languageName);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ViewModel.ViewModelLocator.Cleanup();
        }        
    }
}
