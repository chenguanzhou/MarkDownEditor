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
using System.Windows.Threading;
using Microsoft.Practices.ServiceLocation;
using MarkDownEditor.ViewModel;

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
            editor.TextChanged += Editor_TextChanged;
        }

        private bool ShouldClose = false;
        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainViewModel mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
            
            if (mainViewModel != null)
            {
                if (!ShouldClose && mainViewModel.IsModified)
                {
                    e.Cancel = true;
                    if (await mainViewModel.RequestClosing() == false)
                    {
                        ShouldClose = true;
                        Close();
                    }
                }
            }
        }

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            ScrollAsync();
        }

        private double EditorScrollYRatio { get; set; }

        private void editor_ScrollChanged(object sender, RoutedEventArgs e)
        {
            ScrollChangedEventArgs args = (ScrollChangedEventArgs)e;
            if (args.VerticalOffset == 0)
            {
                EditorScrollYRatio = 0;
                return;
            }

            ScrollViewer viewer = (ScrollViewer)e.OriginalSource;
            EditorScrollYRatio = args.VerticalOffset / viewer.ScrollableHeight;

            ScrollAsync();
        }

        private void ScrollAsync()
        {
            string src = $"scrollTo(0, {EditorScrollYRatio} * document.body.scrollHeight)";
            webBrowser.ExecuteScriptAsync(src);
        }
    }
}
