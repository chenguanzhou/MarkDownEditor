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
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;

namespace MarkDownEditor.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();            
            mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
            webBrowser.Loaded += WebBrowser_Loaded;
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            int cgz = 0;
        }

        public MainViewModel mainViewModel;

        private bool ShouldClose = false;
        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {            
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

        #region Full Screen
        public static readonly DependencyProperty ToggleFullScreenProperty =
            DependencyProperty.Register("ToggleFullScreen",
                                typeof(bool),
                                typeof(MainWindow),
                                new PropertyMetadata(false, ToggleFullScreenPropertyChangedCallback));

        private static WindowState oldWindowState = WindowState.Normal;
        private static void ToggleFullScreenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var metroWindow = (MetroWindow)dependencyObject;
            if (e.OldValue != e.NewValue)
            {
                var fullScreen = (bool)e.NewValue;
                if (fullScreen)
                {
                    oldWindowState = metroWindow.WindowState;
                    metroWindow.WindowState = WindowState.Maximized;
                    metroWindow.UseNoneWindowStyle = true;
                    metroWindow.ShowTitleBar = false;
                    metroWindow.IgnoreTaskbarOnMaximize = true;
                }
                else
                {
                    metroWindow.WindowState = WindowState.Normal;
                    metroWindow.UseNoneWindowStyle = false;
                    metroWindow.ShowTitleBar = true; // <-- this must be set to true
                    metroWindow.IgnoreTaskbarOnMaximize = false;
                    metroWindow.WindowState = oldWindowState;
                }
            }
        }

        public bool ToggleFullScreen
        {
            get { return (bool)GetValue(ToggleFullScreenProperty); }
            set { SetValue(ToggleFullScreenProperty, value); }
        }

        private void ExitFullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullScreen = false;
        }
        #endregion


    }
}
