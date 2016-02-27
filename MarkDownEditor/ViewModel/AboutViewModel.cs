using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using MarkDownEditor.Model;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
            CheckForUpdate();
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
        public string LatestVersionNumber { get; private set; }


        #region Check for update

        private async void CheckForUpdate()
        {
            try
            {
                using (var client = new WebClient())
                {
                    string jsonPath = Path.GetTempFileName() + ".json";
                    await client.DownloadFileTaskAsync(new Uri("https://raw.githubusercontent.com/chenguanzhou/MarkDownEditor/develop/LatestVersion.json"), jsonPath);

                    JObject obj = JObject.Parse(File.ReadAllText(jsonPath));
                    if (obj["AppName"].ToString() != ApplicationName)
                        throw new Exception();
                    var versionObj = obj["LatestVersion"];
                    string latestVersionString = versionObj["Version"].ToString();
                    string descriptionString = versionObj["Description"].ToString();

                    LatestVersionNumber = latestVersionString;
                    if (CompareVersion(latestVersionString, VersionNumber) >0)
                    {
                        
                        var ret = await DialogCoordinator.Instance.ShowMessageAsync(ViewModelLocator.Main,"Update", $"New version {latestVersionString} is released!\nUpdate Info:\n {descriptionString}",
                            MessageDialogStyle.AffirmativeAndNegative,new MetroDialogSettings() { ColorScheme = MetroDialogColorScheme.Accented, AffirmativeButtonText = "Download Now",
                                NegativeButtonText = Properties.Resources.Cancel});
                        if (ret == MessageDialogResult.Affirmative)
                            Process.Start("https://github.com/chenguanzhou/MarkDownEditor/releases");
                    }
                }
            }
            catch(Exception ex)
            {
                ViewModelLocator.Main.StatusBarText = "Fetch App update information failed!";
            }
        }

        #endregion
        int CompareVersion(string left, string right)
        {
            var leftV = left.Split('.').Select(s => int.Parse(s)).ToList();
            var rightV = right.Split('.').Select(s => int.Parse(s)).ToList();

            if (leftV.Count != 4 || rightV.Count != 4)
                throw new Exception();

            for (int i = 0; i < 4; ++i)
            {
                if (leftV[i] > rightV[i])
                    return 1;
                else if (leftV[i] < rightV[i])
                    return -1;
                else
                    continue;
            }

            return 0;
        }
    }
}