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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
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
    public class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SettingsViewModel()
        {
            ThemeManager.ChangeAppStyle(Application.Current, 
                ThemeManager.GetAccent(Properties.Settings.Default.DefaultAccent),
                ThemeManager.GetAppTheme(isNightMode ? "BaseDark" : "BaseLight"));
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public AboutViewModel AboutViewModel { get; } = new AboutViewModel();

        private bool showSettingsControl = false;
        public bool ShowSettingsControl
        {
            get { return showSettingsControl; }
            set
            {
                if (showSettingsControl == value)
                    return;
                showSettingsControl = value;
                RaisePropertyChanged("ShowSettingsControl");
            }
        }

        public ICommand ShowSettingCommand => new RelayCommand(()=> ShowSettingsControl = !ShowSettingsControl);

        #region Environment
        private CultureInfo cultureInfo = new CultureInfo(Properties.Settings.Default.Language);
        public CultureInfo CultureInfo
        {
            get { return cultureInfo; }
            set
            {
                if (cultureInfo?.CompareInfo == value.CompareInfo)
                    return;
                cultureInfo = value;
                Properties.Settings.Default.Language = value?.Name;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("CultureInfo");
                DialogCoordinator.Instance.ShowMessageAsync(ViewModelLocator.Main, Properties.Resources.Warning, Properties.Resources.LanguageSwitch);
            }
        }

        public List<CultureInfo> AllLanguages => App.AllLanguages;

        private bool isNightMode = Properties.Settings.Default.NightMode;
        public bool IsNightMode
        {
            get { return isNightMode; }
            set
            {
                if (isNightMode == value)
                    return;
                isNightMode = value;
                Properties.Settings.Default.NightMode = value;
                Properties.Settings.Default.Save();

                var theme = ThemeManager.DetectAppStyle(Application.Current);
                var appTheme = ThemeManager.GetAppTheme(isNightMode?"BaseDark":"BaseLight");
                ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);

                ViewModelLocator.Main.UpdateCSSFiles();

                RaisePropertyChanged("IsNightMode");
            }
        }

        public class AccentItem
        {
            public string Name { get; set; }
            public Brush ColorBrush { get; set; }
            public ICommand ChangeAccentCommand => new RelayCommand(() =>
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(this.Name), ThemeManager.DetectAppStyle(Application.Current).Item1);
                Properties.Settings.Default.DefaultAccent = this.Name;
                Properties.Settings.Default.Save();
            });
        }
        public List<AccentItem> AccentColors { get; set; } = ThemeManager.Accents.Select(s => new AccentItem() { Name = s.Name, ColorBrush = s.Resources["AccentColorBrush"] as Brush }).ToList();

        public List<string> AllImageStrorageServices => new List<string>
        {
            "SM.MS",
            "IMGUR",
            "Qiniu"
        };

        private int currentImageStrorageServiceIndex = Properties.Settings.Default.CurrentImageStrorageServiceIndex;
        public int CurrentImageStrorageServiceIndex
        {
            get { return currentImageStrorageServiceIndex; }
            set
            {
                if (currentImageStrorageServiceIndex == value)
                    return;

                currentImageStrorageServiceIndex = value;
                Properties.Settings.Default.CurrentImageStrorageServiceIndex = value;
                Properties.Settings.Default.Save();
                
                RaisePropertyChanged("CurrentImageStrorageServiceIndex");
            }
        }

        public string CurrentImageStrorageService => AllImageStrorageServices[CurrentImageStrorageServiceIndex];

        #endregion

        private FontFamily editorFont = new FontFamily(Properties.Settings.Default.EditorFont);
        public FontFamily EditorFont
        {
            get { return editorFont; }
            set
            {
                if (editorFont == value)
                    return;
                editorFont = value;
                Properties.Settings.Default.EditorFont = value.Source;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("EditorFont");
            }
        }

        private int editorFontSize = Properties.Settings.Default.EditorFontSize;
        public int EditorFontSize
        {
            get { return editorFontSize; }
            set
            {
                if (editorFontSize == value)
                    return;
                editorFontSize = value;
                Properties.Settings.Default.EditorFontSize = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("EditorFontSize");
            }
        }

        private bool wordWrap = Properties.Settings.Default.WordWrap;
        public bool WordWrap
        {
            get { return wordWrap; }
            set
            {
                if (wordWrap == value)
                    return;
                wordWrap = value;
                Properties.Settings.Default.WordWrap = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("WordWrap");
            }
        }

        private bool showLineNumbers = Properties.Settings.Default.ShowLineNumbers;
        public bool ShowLineNumbers
        {
            get { return showLineNumbers; }
            set
            {
                if (showLineNumbers == value)
                    return;
                showLineNumbers = value;
                Properties.Settings.Default.ShowLineNumbers = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("ShowLineNumbers");
            }
        }

        private bool showTabs = Properties.Settings.Default.ShowTabs;
        public bool ShowTabs
        {
            get { return showTabs; }
            set
            {
                if (showTabs == value)
                    return;
                showTabs = value;
                Properties.Settings.Default.ShowTabs = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("ShowTabs");
            }
        }

        private bool showSpaces = Properties.Settings.Default.ShowSpaces;
        public bool ShowSpaces
        {
            get { return showSpaces; }
            set
            {
                if (showSpaces == value)
                    return;
                showSpaces = value;
                Properties.Settings.Default.ShowSpaces = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("ShowSpaces");
            }
        }

        private bool showEndOfLine = Properties.Settings.Default.ShowEndOfLine;
        public bool ShowEndOfLine
        {
            get { return showEndOfLine; }
            set
            {
                if (showEndOfLine == value)
                    return;
                showEndOfLine = value;
                Properties.Settings.Default.ShowEndOfLine = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("ShowEndOfLine");
            }
        }
        private bool highlightCurrentLine = Properties.Settings.Default.HighlightCurrentLine;
        public bool HighlightCurrentLine
        {
            get { return highlightCurrentLine; }
            set
            {
                if (highlightCurrentLine == value)
                    return;
                highlightCurrentLine = value;
                Properties.Settings.Default.HighlightCurrentLine = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("HighlightCurrentLine");
            }
        }

        private bool showMathJax = Properties.Settings.Default.ShowMathJax;
        public bool ShowMathJax
        {
            get { return showMathJax; }
            set
            {
                if (showMathJax == value)
                    return;
                showMathJax = value;
                Properties.Settings.Default.ShowMathJax = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("ShowMathJax");
            }
        }

        private bool showColumnRuler = Properties.Settings.Default.ShowColumnRuler;
        public bool ShowColumnRuler
        {
            get { return showColumnRuler; }
            set
            {
                if (showColumnRuler == value)
                    return;
                showColumnRuler = value;
                Properties.Settings.Default.ShowColumnRuler = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("ShowColumnRuler");
            }
        }

        private int rulerPosition = Properties.Settings.Default.RulerPosition;
        public int RulerPosition
        {
            get { return rulerPosition; }
            set
            {
                if (rulerPosition == value)
                    return;
                rulerPosition = value;
                Properties.Settings.Default.RulerPosition = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("RulerPosition");
            }
        }



        private string qiniuACCESS_KEY = Properties.Settings.Default.QiniuACCESS_KEY;
        public string QiniuACCESS_KEY
        {
            get { return qiniuACCESS_KEY; }
            set
            {
                if (qiniuACCESS_KEY == value)
                    return;
                qiniuACCESS_KEY = value;
                Properties.Settings.Default.QiniuACCESS_KEY = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("QiniuACCESS_KEY");
            }
        }


        private string qiniuSECRET_KEY = Properties.Settings.Default.QiniuSECRET_KEY;
        public string QiniuSECRET_KEY
        {
            get { return qiniuSECRET_KEY; }
            set
            {
                if (qiniuSECRET_KEY == value)
                    return;
                qiniuSECRET_KEY = value;
                Properties.Settings.Default.QiniuSECRET_KEY = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("QiniuSECRET_KEY");
            }
        }


        private string qiniuUserDomainName = Properties.Settings.Default.QiniuUserDomainName;
        public string QiniuUserDomainName
        {
            get { return qiniuUserDomainName; }
            set
            {
                if (qiniuUserDomainName == value)
                    return;
                qiniuUserDomainName = value;
                Properties.Settings.Default.QiniuUserDomainName = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("QiniuUserDomainName");
            }
        }

        private string qiniuUserScope = Properties.Settings.Default.QiniuUserScope;
        public string QiniuUserScope
        {
            get { return qiniuUserScope; }
            set
            {
                if (qiniuUserScope == value)
                    return;
                qiniuUserScope = value;
                Properties.Settings.Default.QiniuUserScope = value;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("QiniuUserScope");
            }
        }


    }
}