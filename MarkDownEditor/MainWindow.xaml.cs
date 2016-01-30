using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Diagnostics;
using CefSharp;
using ICSharpCode.AvalonEdit;

namespace MarkDownEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            webBrowser.RequestHandler = new RequestHandler();          
        }       

        private void editor_ScrollChanged(object sender, RoutedEventArgs e)
        {
            ScrollChangedEventArgs args = (ScrollChangedEventArgs)e;
            if (args.VerticalOffset == 0)
                return;

            ScrollViewer viewer = (ScrollViewer)e.OriginalSource;
            double ratio = args.VerticalOffset / viewer.ScrollableHeight;

            string src = $"scrollTo(0, {ratio} * document.body.scrollHeight)";
            webBrowser.ExecuteScriptAsync(src);
        }
    }
}
