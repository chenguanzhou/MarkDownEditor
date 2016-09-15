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
        
        public ICommand ShowAboutCommand => new RelayCommand(() => ShowAboutControl = !ShowAboutControl);
        public ICommand ClickSourceCodeWebCommand => new RelayCommand(() => Process.Start("https://github.com/chenguanzhou/MarkDownEditor"));
        public ICommand CheckForUpdateCommand => new RelayCommand(()=> 
        {
            Properties.Settings.Default.DoNotRemindUpdate = false;
            Properties.Settings.Default.Save();
            CheckForUpdate();
        });


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
                    await client.DownloadFileTaskAsync(new Uri(@"https://raw.githubusercontent.com/chenguanzhou/MarkDownEditor/master/LatestVersion.json"), jsonPath);

                    JObject obj = JObject.Parse(File.ReadAllText(jsonPath));
                    if (obj["AppName"].ToString() != ApplicationName)
                        throw new Exception();
                    var versionObj = obj["LatestVersion"];
                    string latestVersionString = versionObj["Version"].ToString();
                    string descriptionString = versionObj["Description"].ToString();

                    LatestVersionNumber = latestVersionString;
                    if (CompareVersion(latestVersionString, VersionNumber) >0)
                    {
                        if (CompareVersion(latestVersionString, Properties.Settings.Default.LastLatestVersion) > 0)
                        {
                            Properties.Settings.Default.DoNotRemindUpdate = false;
                            Properties.Settings.Default.LastLatestVersion = latestVersionString;
                            Properties.Settings.Default.Save();
                        }

                        if (!Properties.Settings.Default.DoNotRemindUpdate)
                        {
                            NotifyUpdate(latestVersionString, descriptionString);
                        }
                    }
                }
            }
            catch(Exception)
            {
                ViewModelLocator.Main.StatusBarText = Properties.Resources.UpdateFailed;
            }
        }

        #endregion
        int CompareVersion(string left, string right)
        {
            if (string.IsNullOrEmpty(left))
                return 1;
            else if (string.IsNullOrEmpty(right))
                return -1;
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

        private async void NotifyUpdate(string latestVersionString, string descriptionString)
        {
            var ret = await DialogCoordinator.Instance.ShowMessageAsync(ViewModelLocator.Main, Properties.Resources.Update, 
                $"{Properties.Resources.NewVersionIsReleased}\n\n{Properties.Resources.LatestVersion}:  {latestVersionString}\n{Properties.Resources.CurrentVersion}:  {VersionNumber}\n{Properties.Resources.UpdateInfo}:\n{descriptionString}",
                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings()
                    {
                        ColorScheme = MetroDialogColorScheme.Accented,
                        AffirmativeButtonText = Properties.Resources.DownloadNow,
                        NegativeButtonText = Properties.Resources.Cancel,
                        FirstAuxiliaryButtonText = Properties.Resources.DoNotRemindForThisUpdate
                    });
            if (ret == MessageDialogResult.Affirmative)
                Process.Start("https://github.com/chenguanzhou/MarkDownEditor/releases");
            else if (ret == MessageDialogResult.FirstAuxiliary)
            {
                Properties.Settings.Default.DoNotRemindUpdate = true;
                Properties.Settings.Default.LastLatestVersion = latestVersionString;
                Properties.Settings.Default.Save();
            }
        }
    }
}