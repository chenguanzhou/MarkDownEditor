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
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

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
    }
}