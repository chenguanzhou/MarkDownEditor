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
    public class FindReplaceViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public FindReplaceViewModel()
        {
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        private bool showFindReplaceControl = false;
        public bool ShowFindReplaceControl
        {
            get { return showFindReplaceControl; }
            set
            {
                if (showFindReplaceControl == value)
                    return;
                showFindReplaceControl = value;
                if (showFindReplaceControl)
                {
                    ViewModelLocator.Main.SourceCode.TextChanged += SourceCode_TextChanged;                    
                    UpdateCommandsState();
                }
                else
                    ViewModelLocator.Main.SourceCode.TextChanged -= SourceCode_TextChanged;

                RaisePropertyChanged("ShowFindReplaceControl");
            }
        }

        private bool isFindPreviousEnabled = false;
        public bool IsFindPreviousEnabled
        {
            get { return isFindPreviousEnabled; }
            set
            {
                if (isFindPreviousEnabled == value)
                    return;
                isFindPreviousEnabled = value;
                RaisePropertyChanged("IsFindPreviousEnabled");
            }
        }

        private bool isFindNextEnabled = false;
        public bool IsFindNextEnabled
        {
            get { return isFindNextEnabled; }
            set
            {
                if (isFindNextEnabled == value)
                    return;
                isFindNextEnabled = value;
                RaisePropertyChanged("IsFindNextEnabled");
            }
        }

        private bool isReplaceEnabled = false;
        public bool IsReplaceEnabled
        {
            get { return isReplaceEnabled; }
            set
            {
                if (isReplaceEnabled == value)
                    return;
                isReplaceEnabled = value;
                RaisePropertyChanged("IsReplaceEnabled");
            }
        }

        private bool isReplaceAllEnabled = false;
        public bool IsReplaceAllEnabled
        {
            get { return isReplaceAllEnabled; }
            set
            {
                if (isReplaceAllEnabled == value)
                    return;
                isReplaceAllEnabled = value;
                RaisePropertyChanged("IsReplaceAllEnabled");
            }
        }

        private bool isMatchCase = false;
        public bool IsMatchCase
        {
            get { return isMatchCase; }
            set
            {
                if (isMatchCase == value)
                    return;
                isMatchCase = value;
                RaisePropertyChanged("IsMatchCase");
                UpdateCommandsState();
            }
        }

        private bool isMatchWholeWord = false;
        public bool IsMatchWholeWord
        {
            get { return isMatchWholeWord; }
            set
            {
                if (isMatchWholeWord == value)
                    return;
                isMatchWholeWord = value;
                RaisePropertyChanged("IsMatchWholeWord");
                UpdateCommandsState();
            }
        }

        private bool useRegExp = false;
        public bool UseRegExp
        {
            get { return useRegExp; }
            set
            {
                if (useRegExp == value)
                    return;
                useRegExp = value;
                RaisePropertyChanged("UseRegExp");
                UpdateCommandsState();
            }
        }

        private bool useWildcards = false;
        public bool UseWildcards
        {
            get { return useWildcards; }
            set
            {
                if (useWildcards == value)
                    return;
                useWildcards = value;
                RaisePropertyChanged("UseWildcards");
                UpdateCommandsState();
            }
        }

        private string searchText = "";
        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (searchText == value)
                    return;
                searchText = value;
                RaisePropertyChanged("SearchText");
                UpdateCommandsState();
            }
        }

        private string replaceText = "";
        public string ReplaceText
        {
            get { return replaceText; }
            set
            {
                if (replaceText == value)
                    return;
                replaceText = value;
                RaisePropertyChanged("ReplaceText");
                UpdateCommandsState();
            }
        }

        public class MatchObject
        {
            public int Start { get; set; }
            public int Length { get; set; }
        }
        public List<MatchObject> MatchedCollection
        {
            get; set;
        }

        public int currentMatch = 0;
        public int CurrentMatch
        {
            get { return currentMatch; }
            set
            {
                currentMatch = value;
                if (MatchedCollection == null || MatchedCollection.Count == 0)
                {
                    IsReplaceEnabled = IsReplaceAllEnabled = false;
                    return;
                }

                IsFindPreviousEnabled = CurrentMatch > 0;
                IsFindNextEnabled = CurrentMatch < MatchedCollection.Count - 1;

                var match = MatchedCollection[currentMatch];
                ViewModelLocator.Main.SelectionStart = 0;
                ViewModelLocator.Main.SelectionLength = match.Length;
                ViewModelLocator.Main.SelectionStart = match.Start;
                ViewModelLocator.Main.ScrollToSelectionStart = !ViewModelLocator.Main.ScrollToSelectionStart;

                IsReplaceEnabled = IsReplaceAllEnabled = true;
            }
        }
    
        #region Commands
        public ICommand SwitchFindReplaceCommand => new RelayCommand(() => ShowFindReplaceControl = !ShowFindReplaceControl);
        public ICommand ShowFindReplaceCommand => new RelayCommand(() => ShowFindReplaceControl = true);
        public ICommand HideFindReplaceCommand => new RelayCommand(() => ShowFindReplaceControl = false);


        public ICommand FindPreviousCommand => new RelayCommand(() =>
        {
            CurrentMatch--;
        });

        public ICommand FindNextCommand => new RelayCommand(() =>
        {
            CurrentMatch++;
        });

        public ICommand ReplaceCommand => new RelayCommand(async () =>
        {
            if (string.IsNullOrEmpty(SearchText))
                return;

            int oldStartOffset = MatchedCollection[CurrentMatch].Start;
            int oldLength = MatchedCollection[CurrentMatch].Length;

            ViewModelLocator.Main.SourceCode.TextChanged -= SourceCode_TextChanged;
            ViewModelLocator.Main.SourceCode.Replace(MatchedCollection[CurrentMatch].Start, MatchedCollection[CurrentMatch].Length, ReplaceText);
            ViewModelLocator.Main.SourceCode.TextChanged += SourceCode_TextChanged;


            MatchedCollection.RemoveAt(CurrentMatch);
            for (int i = CurrentMatch; i < MatchedCollection.Count; ++i)
            {
                MatchedCollection[i].Start += ReplaceText.Length - oldLength;
            }

            if (CurrentMatch == MatchedCollection.Count)
            {
                await DialogCoordinator.Instance.ShowMessageAsync(ViewModelLocator.Main, Properties.Resources.Completed, "Completed Replace!",
                    MessageDialogStyle.Affirmative, new MetroDialogSettings() { ColorScheme = MetroDialogColorScheme.Accented });
                CurrentMatch = 0;
            }

            CurrentMatch = CurrentMatch;
        });
       
        public ICommand ReplaceAllCommand => new RelayCommand(() =>
        {
            if (string.IsNullOrEmpty(SearchText))
                return;

            ViewModelLocator.Main.SourceCode.TextChanged -= SourceCode_TextChanged;
            var content = ViewModelLocator.Main.SourceCode.Text;
            for (int i= 0; i < MatchedCollection.Count; )
            {
                int oldLength = MatchedCollection[i].Length;
                //ViewModelLocator.Main.SourceCode.Replace(MatchedCollection[i].Start, MatchedCollection[i].Length, ReplaceText);
                content = content.Remove(MatchedCollection[i].Start, MatchedCollection[i].Length)
                .Insert(MatchedCollection[i].Start, ReplaceText);

                MatchedCollection.RemoveAt(i);
                for (int j = i; j < MatchedCollection.Count; ++j)
                {
                    MatchedCollection[j].Start += ReplaceText.Length - oldLength;
                }
            }

            ViewModelLocator.Main.SourceCode.Text = content;
            ViewModelLocator.Main.SourceCode.TextChanged += SourceCode_TextChanged;
            UpdateCommandsState();
        });
        #endregion //Commands


        #region Functions
        private void UpdateCommandsState()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                MatchedCollection = null;
                ClearFindState();
            }
            else
            {
                Regex regex = GetRegEx(SearchText);
                MatchedCollection = regex.Matches(ViewModelLocator.Main.SourceCode.Text).Cast<Match>()
                    .Select(s=>new MatchObject() {Start=s.Index,Length=s.Length }).ToList();
                
                if (MatchedCollection.Count == 0)//nothing matched
                {
                    ClearFindState();
                    ViewModelLocator.Main.SelectionLength = 0;
                    return;
                }
                else
                {
                    CurrentMatch = 0;
                    IsFindPreviousEnabled = false;
                    IsFindNextEnabled = MatchedCollection.Count > 1;
                    IsReplaceEnabled = IsReplaceAllEnabled = MatchedCollection.Count > 0;
                }
            }   
        }

        private Regex GetRegEx(string textToFind)
        {
            RegexOptions options = RegexOptions.None;
            if (!IsMatchCase)
                options |= RegexOptions.IgnoreCase;

            if (UseRegExp)
            {
                return new Regex(textToFind, options);
            }
            else
            {
                string pattern = Regex.Escape(textToFind);
                if (UseWildcards)
                    pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
                if (IsMatchWholeWord)
                    pattern = "\\b" + pattern + "\\b";
                return new Regex(pattern, options);
            }
        }

        private void ClearFindState()
        {
            IsFindPreviousEnabled = false;
            IsFindNextEnabled = false;
            IsReplaceEnabled = false;
            IsReplaceAllEnabled = false;
            CurrentMatch = 0;
        }

        private void SourceCode_TextChanged(object sender, EventArgs e)
        {
            //UpdateCommandsState();
            this.ShowFindReplaceControl = false;
        }

        #endregion //Functions
    }
}