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

        private FontFamily editorFont = new FontFamily("Consolas");
        public FontFamily EditorFont
        {
            get { return editorFont; }
            set
            {
                if (editorFont == value)
                    return;
                editorFont = value;
                RaisePropertyChanged("EditorFont");
            }
        }

        private int editorFontSize = 16;
        public int EditorFontSize
        {
            get { return editorFontSize; }
            set
            {
                if (editorFontSize == value)
                    return;
                editorFontSize = value;
                RaisePropertyChanged("EditorFontSize");
            }
        }

        private bool wordWrap = false;
        public bool WordWrap
        {
            get { return wordWrap; }
            set
            {
                if (wordWrap == value)
                    return;
                wordWrap = value;
                RaisePropertyChanged("WordWrap");
            }
        }

        private bool showLineNumbers = true;
        public bool ShowLineNumbers
        {
            get { return showLineNumbers; }
            set
            {
                if (showLineNumbers == value)
                    return;
                showLineNumbers = value;
                RaisePropertyChanged("ShowLineNumbers");
            }
        }

        private bool showTabs = false;
        public bool ShowTabs
        {
            get { return showTabs; }
            set
            {
                if (showTabs == value)
                    return;
                showTabs = value;
                RaisePropertyChanged("ShowTabs");
            }
        }

        private bool showSpaces = false;
        public bool ShowSpaces
        {
            get { return showSpaces; }
            set
            {
                if (showSpaces == value)
                    return;
                showSpaces = value;
                RaisePropertyChanged("ShowSpaces");
            }
        }

        private bool showEndOfLine = false;
        public bool ShowEndOfLine
        {
            get { return showEndOfLine; }
            set
            {
                if (showEndOfLine == value)
                    return;
                showEndOfLine = value;
                RaisePropertyChanged("ShowEndOfLine");
            }
        }

        private bool highlightCurrentLine = true;
        public bool HighlightCurrentLine
        {
            get { return highlightCurrentLine; }
            set
            {
                if (highlightCurrentLine == value)
                    return;
                highlightCurrentLine = value;
                RaisePropertyChanged("HighlightCurrentLine");
            }
        }

        private bool showColumnRuler = true;
        public bool ShowColumnRuler
        {
            get { return showColumnRuler; }
            set
            {
                if (showColumnRuler == value)
                    return;
                showColumnRuler = value;
                RaisePropertyChanged("ShowColumnRuler");
            }
        }

        private int rulerPosition = 80;
        public int RulerPosition
        {
            get { return rulerPosition; }
            set
            {
                if (rulerPosition == value)
                    return;
                rulerPosition = value;
                RaisePropertyChanged("RulerPosition");
            }
        }
    }
}