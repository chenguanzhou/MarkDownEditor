using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

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
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            sourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => UpdatePreview());
            UpdatePreview();
            sourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => IsModified = true);
        }

        private string markdownSourceTempPath = Path.GetTempFileName();
        private string previewSourceTempPath = Path.GetTempFileName() + ".html";

        public string Title => DocumentTitle + (IsModified?"(*)":"") +" ---- MarkDown Editor Alpha";

        public string PreviewSource => previewSourceTempPath;

        public string documentTitle = "Untitled";
        public string DocumentTitle
        {
            get { return documentTitle; }
            set
            {
                if (documentTitle == value)
                    return;
                documentTitle = value;
                RaisePropertyChanged("DocumentTitle");
                RaisePropertyChanged("Title");
            }
        }

        public string documentPath = null;
        public string DocumentPath
        {
            get { return documentPath; }
            set
            {
                if (documentPath == value)
                    return;
                documentPath = value;
                RaisePropertyChanged("DocumentPath");
            }
        }

        public bool isModified = false;
        public bool IsModified
        {
            get { return isModified; }
            set
            {
                if (isModified == value)
                    return;
                isModified = value;
                RaisePropertyChanged("IsModified");
                RaisePropertyChanged("Title");
            }
        }

        private TextDocument sourceCode = new TextDocument();
        public TextDocument SourceCode
        {
            get { return sourceCode; }
            set
            {
                if (sourceCode == value)
                    return;
                sourceCode = value;
                RaisePropertyChanged("SourceCode");
            }
        }

        private string editorFont = "Consolas";
        public string EditorFont
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

        public double ScrollbarPos { get; set; }

        public Dictionary<string,string> MarkDownType => new Dictionary<string, string>()
        {
            { "Markdown", "markdown" },
            { "Markdown_Strict", "markdown_strict"},
            { "Markdown_Github", "markdown_github" },
            { "Markdown_mmd", "markdown_mmd" }
        };

        private string currentMarkdownTypeText = "Markdown";
        public string CurrentMarkdownTypeText
        {
            get { return currentMarkdownTypeText; }
            set
            {
                if (currentMarkdownTypeText == value)
                    return;
                currentMarkdownTypeText = value;
                RaisePropertyChanged("CurrentMarkdownTypeText");
                UpdatePreview();
            }
        }

        #region Document Commands
        public ICommand NewDocumentCommand => new RelayCommand(async () =>
        {
            Action CreateNewDoc = () =>
            {
                SourceCode.Text = "";
                DocumentPath = null;
                DocumentTitle = "Untitled";
                IsModified = false;
            };

            if (IsModified)
            {
                var ret = await DialogCoordinator.Instance.ShowMessageAsync(this, "New Document", "Would you want to save your changes?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                    new MetroDialogSettings() { AffirmativeButtonText = "Save", NegativeButtonText = "Don't save", FirstAuxiliaryButtonText = "Cancle" });
                if (ret == MessageDialogResult.Affirmative)
                {

                }
                else if (ret == MessageDialogResult.Negative)
                    CreateNewDoc();
            }
            else
                CreateNewDoc();
        });

        public ICommand OpenDocumentCommand => new RelayCommand(async () =>
        {
            Action OpenDoc = async () =>
            {
                var dlg = new OpenFileDialog();
                dlg.Filter = "";
                if (dlg.ShowDialog()==true)
                {
                    StreamReader sr = new StreamReader(dlg.FileName);
                    SourceCode.Text = await sr.ReadToEndAsync();
                    sr.Close();
                    SourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => UpdatePreview());
                    UpdatePreview();
                    SourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => IsModified = true);
                    DocumentPath = dlg.FileName;
                    DocumentTitle = Path.GetFileName(dlg.FileName);
                    IsModified = false;
                }                
            };

            if (IsModified)
            {
                var ret = await DialogCoordinator.Instance.ShowMessageAsync(this, "New Document", "Would you want to save your changes?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                    new MetroDialogSettings() { AffirmativeButtonText = "Save", NegativeButtonText = "Don't save", FirstAuxiliaryButtonText = "Cancle" });
                if (ret == MessageDialogResult.Affirmative)
                {

                }
                else if (ret == MessageDialogResult.Negative)
                    OpenDoc();
            }
            else
                OpenDoc();
        });

        public ICommand SaveDocumentCommand => new RelayCommand(() =>
        {

        });

        public ICommand SaveAsDocumentCommand => new RelayCommand(() =>
        {

        });

        #endregion


        #region Editor Commands
        public ICommand BoldCommand => new RelayCommand<object>((obj) => 
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            if (editor.SelectionStart>1 && editor.SelectionStart+editor.SelectionLength+1 < sourceCode.TextLength)
            {
                if (sourceCode.GetText(editor.SelectionStart-2,2)=="**" && sourceCode.GetText(editor.SelectionStart+ editor.SelectionLength, 2) == "**")
                {
                    sourceCode.Remove(editor.SelectionStart + editor.SelectionLength, 2);
                    sourceCode.Remove(editor.SelectionStart - 2, 2);
                    return;
                }
            }

            if (editor.SelectionLength == 0)
            {
                int OriginalStart = editor.SelectionStart;
                sourceCode.Insert(editor.SelectionStart,$"**{Properties.Resources.EmptyBoldAreaText}**");
                editor.SelectionStart = OriginalStart + 2;
                editor.SelectionLength = Properties.Resources.EmptyBoldAreaText.Length;
                return;
            }
            else
            {
                int OriginalStart = editor.SelectionStart;
                sourceCode.Insert(editor.SelectionStart, "**");
                sourceCode.Insert(editor.SelectionStart+ editor.SelectionLength, "**");
            }
        });

        public ICommand ItalicCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            if (editor.SelectionStart > 0 && editor.SelectionStart + editor.SelectionLength < sourceCode.TextLength)
            {
                if (sourceCode.GetText(editor.SelectionStart - 1, 1) == "*" && sourceCode.GetText(editor.SelectionStart + editor.SelectionLength, 1) == "*")
                {
                    sourceCode.Remove(editor.SelectionStart + editor.SelectionLength, 1);
                    sourceCode.Remove(editor.SelectionStart - 1, 1);
                    return;
                }
            }

            if (editor.SelectionLength == 0)
            {
                int OriginalStart = editor.SelectionStart;
                sourceCode.Insert(editor.SelectionStart, $"*{Properties.Resources.EmptyItalicAreaText}*");
                editor.SelectionStart = OriginalStart + 1;
                editor.SelectionLength = Properties.Resources.EmptyItalicAreaText.Length;
                return;
            }
            else
            {
                int OriginalStart = editor.SelectionStart;
                sourceCode.Insert(editor.SelectionStart, "*");
                sourceCode.Insert(editor.SelectionStart + editor.SelectionLength, "*");
            }
        });

        public ICommand QuoteCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;

            if (editor.SelectionLength == 0)
                sourceCode.Insert(editor.SelectionStart, "\r\n>");
            else
            {
                var selectedText = sourceCode.GetText(editor.SelectionStart, editor.SelectionLength);
                selectedText = selectedText.Replace("\r\n", " ");
                selectedText = "\r\n\r\n>" + selectedText;
                sourceCode.Replace(editor.SelectionStart, editor.SelectionLength, selectedText,OffsetChangeMappingType.RemoveAndInsert);
            }
        });

        public ICommand HyperlinkCommand => new RelayCommand<object>(async (obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            string url = await DialogCoordinator.Instance.ShowInputAsync(this, "Hyperlink", "Please input an URL",new MetroDialogSettings() { DefaultText = "http://"});
            if (url != null)
            {
                if (editor.SelectionLength == 0)
                {
                    var toInsert = $"[{Properties.Resources.EmptyHyperlinkDescription}]({url})";
                    sourceCode.Insert(editor.SelectionStart, toInsert, AnchorMovementType.BeforeInsertion);
                    editor.SelectionStart += 1;
                    editor.SelectionLength = Properties.Resources.EmptyHyperlinkDescription.Length;
                }
                else
                {
                    var toInsert = $"[{editor.SelectedText}]({url})";
                    sourceCode.Replace(editor.SelectionStart, editor.SelectionLength, toInsert, OffsetChangeMappingType.RemoveAndInsert);
                    editor.SelectionLength = 0;
                }
            }
        });

        public ICommand GeneralCodeCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;

            if (editor.SelectionStart > 0)
            {
                if (editor.SelectedText.Contains("\t") || editor.SelectedText.Contains("    "))
                {
                    var selectedText = editor.SelectedText.Replace("\r\n", "\n");
                    string[] lines = selectedText.Split('\n');

                    int count = 0;
                    for (int i = 0; i < lines.Length; ++i)
                    {
                        if (lines[i].StartsWith("\t") )
                        {
                            ++count;
                            lines[i] = lines[i].Substring(1, lines[i].Length-1);
                        }
                        else  if (lines[i].StartsWith("    "))
                        {
                            ++count;
                            lines[i] = lines[i].Substring(4, lines[i].Length-4);
                        }
                    }
                    if (count != 0)
                    {
                        sourceCode.Replace(editor.SelectionStart,editor.SelectionLength, string.Join("\r\n", lines),OffsetChangeMappingType.RemoveAndInsert);
                        return;
                    }
                }
                else
                {
                    if (sourceCode.GetText(editor.SelectionStart - 1, 1) == "`" && sourceCode.GetText(editor.SelectionStart + editor.SelectionLength, 1) == "`")
                    {
                        sourceCode.Remove(editor.SelectionStart + editor.SelectionLength, 1);
                        sourceCode.Remove(editor.SelectionStart - 1, 1);
                        return;
                    }
                }
            }

            if (editor.SelectionLength == 0)
            {
                var toInsert = $"`{Properties.Resources.EmptyCode}`";
                sourceCode.Insert(editor.SelectionStart, toInsert, AnchorMovementType.BeforeInsertion);
                editor.SelectionStart += 1;
                editor.SelectionLength = Properties.Resources.EmptyCode.Length;
            }
            else if (editor.SelectedText.Contains("\n"))//Multiline
            {
                var selectedText = sourceCode.GetText(editor.SelectionStart, editor.SelectionLength);
                selectedText = selectedText.Replace("\r\n", "\r\n\t");

                selectedText = $"\r\n\t{selectedText}\r\n\r\n";
                if (editor.SelectionStart > 0 && sourceCode.GetText(editor.SelectionStart - 1, 1) != "\n")
                    selectedText = "\r\n\t" + selectedText;

                sourceCode.Replace(editor.SelectionStart, editor.SelectionLength, selectedText, OffsetChangeMappingType.RemoveAndInsert);
            }
            else//Single line
            {
                int oldSelectionStart = editor.SelectionStart;
                int oldLength = editor.SelectedText.Length;
                sourceCode.Replace(editor.SelectionStart,editor.SelectionLength, $"`{editor.SelectedText}`", OffsetChangeMappingType.RemoveAndInsert);
                editor.SelectionStart = oldSelectionStart + 1;
                editor.SelectionLength = oldLength;
            }
        });

        public ICommand SeparateLineCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            sourceCode.Replace(editor.SelectionStart, editor.SelectionLength, "\r\n\r\n--------\r\n\r\n");
        });

        #endregion

        private void UpdatePreview()
        {
            StreamWriter sw = new StreamWriter(markdownSourceTempPath);
            sw.Write(SourceCode.Text);
            sw.Close();

            Process process = new Process();
            process.StartInfo.FileName = "pandoc";
            process.StartInfo.Arguments = $"\"{markdownSourceTempPath}\" -f {MarkDownType[CurrentMarkdownTypeText]} -t html --ascii -s -H theme.css -o \"{previewSourceTempPath}\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            RaisePropertyChanged("PreviewSource");
        }
    }
}