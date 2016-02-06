using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using MarkDownEditor.Model;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MarkDownEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AboutViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public AboutViewModel()
        {
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        private bool showAboutControl = false;
        public bool ShowAboutControl
        {
            get { return showAboutControl; }
            set
            {
                if (showAboutControl == value)
                    return;
                showAboutControl = value;
                RaisePropertyChanged("ShowAboutControl");
            }
        }
        
        public ICommand ShowAboutCommand => new RelayCommand(() => ShowAboutControl = true);
        public ICommand ClickSourceCodeWebCommand => new RelayCommand(() => Process.Start("https://github.com/chenguanzhou/MarkDownEditor"));


        public string ApplicationName => Assembly.GetEntryAssembly().GetCustomAttributesData()[3].ConstructorArguments[0].ToString().Replace("\"", "");
        public string VersionNumber => Assembly.GetEntryAssembly().GetName().Version.ToString();
    }
}