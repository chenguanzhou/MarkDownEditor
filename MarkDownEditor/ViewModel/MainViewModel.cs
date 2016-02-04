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
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase);

            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Properties.Settings.Default.DefaultAccent), ThemeManager.DetectAppStyle(Application.Current).Item1);
            CurrentMarkdownTypeText = Properties.Settings.Default.MarkdownProcessor;

            sourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => UpdatePreview());
            UpdatePreview();
            sourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => IsModified = CanUndo);

            var line = SourceCode.GetLineByOffset(CaretOffset);
            CurrentCaretStatisticsInfo = $"Ln: {line.LineNumber}    Col: {CaretOffset - line.Offset}";
        }

        public override void Cleanup()
        {
            base.Cleanup();
            File.Delete(markdownSourceTempPath);
            File.Delete(previewSourceTempPath);
        }

        private string markdownSourceTempPath = Path.GetTempFileName();
        private string previewSourceTempPath = Path.GetTempFileName() + ".html";

        public string Title => DocumentTitle + (IsModified ? "(*)" : "") + " ---- MarkDown Editor Alpha";

        public string PreviewSource => previewSourceTempPath;

        public bool showPreview = true;
        public bool IsShowPreview
        {
            get { return showPreview; }
            set
            {
                if (showPreview == value)
                    return;
                showPreview = value;
                PreviewWidth = showPreview ? "*" : "0";
                if (showPreview)
                    UpdatePreview();
                RaisePropertyChanged("IsShowPreview");
            }
        }

        public bool isSynchronize = true;
        public bool IsSynchronize
        {
            get { return isSynchronize; }
            set
            {
                if (isSynchronize == value)
                    return;
                isSynchronize = value;
                RaisePropertyChanged("IsSynchronize");
            }
        }


        public bool isReadingMode = false;
        public bool IsReadingMode
        {
            get { return isReadingMode; }
            set
            {
                if (isReadingMode == value)
                    return;
                isReadingMode = value;
                RaisePropertyChanged("IsReadingMode");
                SourceCodeWidth = isReadingMode ? "0":"*";
            }
        }

        public string sourceCodeWidth = "*";
        public string SourceCodeWidth
        {
            get { return sourceCodeWidth; }
            set
            {
                if (sourceCodeWidth == value)
                    return;
                sourceCodeWidth = value;
                RaisePropertyChanged("SourceCodeWidth");
            }
        }

        public string previewWidth = "*";
        public string PreviewWidth
        {
            get { return previewWidth; }
            set
            {
                if (previewWidth == value)
                    return;
                previewWidth = value;
                RaisePropertyChanged("PreviewWidth");
            }
        }

        public string documentTitle = Properties.Resources.UntitledTitle;
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

        public bool isBrowserInitialized = false;
        public bool IsBrowserInitialized
        {
            get { return isBrowserInitialized; }
            set
            {
                if (isBrowserInitialized == value)
                    return;
                isBrowserInitialized = value;
                RaisePropertyChanged("IsBrowserInitialized");

                if (isBrowserInitialized)
                {
                    LoadDefaultDocument();                    
                }
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

        private Task statusBarCancelTask = null;
        private string statusBarText = "";
        public string StatusBarText
        {
            get { return statusBarText; }
            set
            {
                if (statusBarText == value)
                    return;               

                statusBarText = value;
                if (!string.IsNullOrEmpty(value))
                {
                    statusBarCancelTask = Task.Run(
                    () =>
                    {
                        string currentText = value;
                        System.Threading.Thread.Sleep(5000);
                        if (StatusBarText == value)
                            StatusBarText = "";
                    });
                }                
                RaisePropertyChanged("StatusBarText");
            }
        }

        private int caretOffset;
        public int CaretOffset
        {
            get { return caretOffset; }
            set
            {
                if (caretOffset == value)
                    return;
                caretOffset = value;
                var line = SourceCode.GetLineByOffset(CaretOffset);
                CurrentCaretStatisticsInfo = $"Ln: {line.LineNumber}    Col: {CaretOffset- line.Offset}";
                RaisePropertyChanged("CaretOffset");
            }
        }

        private string currentCaretStatisticsInfo = "";
        public string CurrentCaretStatisticsInfo
        {
            get { return currentCaretStatisticsInfo; }
            set
            {
                if (currentCaretStatisticsInfo == value)
                    return;
                currentCaretStatisticsInfo = value;
                RaisePropertyChanged("CurrentCaretStatisticsInfo");
            }
        }
                
        private string documrntStatisticsInfo = "";
        public string DocumrntStatisticsInfo
        {
            get { return documrntStatisticsInfo; }
            set
            {
                if (documrntStatisticsInfo == value)
                    return;
                documrntStatisticsInfo = value;
                RaisePropertyChanged("DocumrntStatisticsInfo");
            }
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

        public double ScrollbarPos { get; set; }

        public class ExportFileType
        {
            public ExportFileType(string sourceCodePath)
            {
                SourceCodePath = sourceCodePath;
            }
            public string Name { get; set; }            
            public string Filter { get; set; }
            public string ToolTip { get; set; }
            public string SourceCodePath { get; set; }

            public ICommand ExportCommand => new RelayCommand(async () =>
            {
                var context = ServiceLocator.Current.GetInstance<MainViewModel>();
                if (DocumentExporter.CanExport(Name) == false)
                {
                    await DialogCoordinator.Instance.ShowMessageAsync(context, 
                        "Error", $"File of {Name} can't be exported in this version!");
                    return;
                }
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title = "Export To " + "Name";
                dlg.FileName = context.DocumentPath==null?context.DocumentTitle
                        :Path.GetFileNameWithoutExtension(context.DocumentPath);
                dlg.Filter = Filter;
                if (dlg.ShowDialog() == true)
                {
                    var progress = await DialogCoordinator.Instance.ShowProgressAsync(context, "Export", "Exporting");
                    progress.SetIndeterminate();
                    try
                    {
                        DocumentExporter.Export(Name, context.CurrentMarkdownTypeText, SourceCodePath, dlg.FileName);
                        await progress.CloseAsync();
                        var ret = await DialogCoordinator.Instance.ShowMessageAsync(context,
                            "Completed", $"Export file :\"{dlg.FileName}\" successfully!\nOpen it right now?",
                            MessageDialogStyle.AffirmativeAndNegative,new MetroDialogSettings() { AffirmativeButtonText="Open",
                            NegativeButtonText="Cancel"});
                        if (ret == MessageDialogResult.Affirmative)
                        {
                            Process.Start(dlg.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        await progress.CloseAsync();
                        await DialogCoordinator.Instance.ShowMessageAsync(context,
                            "Error", $"Failed to export!\nDetail: {ex.Message}");
                    }
                }
            });
        }

        public List<ExportFileType> ExportFileTypes => new List<ExportFileType>()
        {
            new ExportFileType(markdownSourceTempPath) {Name="Plain Html", ToolTip=Properties.Resources.TypePlainHtmlToolTip, Filter=Properties.Resources.TypePlainHtmlFilter },
            new ExportFileType(markdownSourceTempPath) {Name="Html" , ToolTip=Properties.Resources.TypeHtmlToolTip, Filter=Properties.Resources.TypeHtmlFilter },
            new ExportFileType(markdownSourceTempPath) {Name="RTF" , ToolTip=Properties.Resources.TypeRTFFilter, Filter=Properties.Resources.TypeRTFFilter },
            new ExportFileType(markdownSourceTempPath) {Name="Docx" , ToolTip=Properties.Resources.TypeDocxToolTip, Filter=Properties.Resources.TypeDocxFilter },
            new ExportFileType(markdownSourceTempPath) {Name="Epub" , ToolTip=Properties.Resources.TypeEpubToolTip, Filter=Properties.Resources.TypeEpubFilter },
            new ExportFileType(markdownSourceTempPath) {Name="Latex", ToolTip=Properties.Resources.TypeLatexToolTip, Filter=Properties.Resources.TypeLatexFilter }
        };

        public Dictionary<string, string> MarkDownType => new Dictionary<string, string>()
        {
            { "Markdown", "markdown" },
            { "Strict Markdown", "markdown_strict"},
            { "GitHub Flavored Markdown", "markdown_github" },
            { "PHP Markdown Extra", "markdown_mmd" },
            { "MultiMarkdown", "markdown_mmd" }
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
                Properties.Settings.Default.MarkdownProcessor = currentMarkdownTypeText;
                Properties.Settings.Default.Save();
                UpdatePreview();
            }
        }

        public class AccentItem
        {
            public string Name { get; set; }
            public Brush ColorBrush { get; set; }
            public ICommand ChangeAccentCommand => new RelayCommand(()=> 
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(this.Name), ThemeManager.DetectAppStyle(Application.Current).Item1);
                Properties.Settings.Default.DefaultAccent = this.Name;
                Properties.Settings.Default.Save();
            });
        }
        public List<AccentItem> AccentColors { get; set; } = ThemeManager.Accents.Select(s=>new AccentItem() { Name=s.Name,ColorBrush= s.Resources["AccentColorBrush"] as Brush }).ToList();

        #region Document Commands
        public ICommand NewDocumentCommand => new RelayCommand(async () =>
        {
            Action CreateNewDoc = () =>
            {
                SourceCode = new TextDocument();
                SourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => UpdatePreview());
                UpdatePreview();
                SourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => IsModified = CanUndo);

                DocumentPath = null;
                DocumentTitle = Properties.Resources.UntitledTitle;
                IsModified = false;
                StatusBarText = "New document created";
            };

            if (IsModified)
            {
                var ret = await DialogCoordinator.Instance.ShowMessageAsync(this, "Unsaved Changes", "Would you want to save your changes?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                    new MetroDialogSettings() { AffirmativeButtonText = "Save", NegativeButtonText = "Don't save", FirstAuxiliaryButtonText = "Cancle" });
                if (ret == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        if (Save() == false)
                            return;
                    }
                    catch (Exception ex)
                    {
                        await DialogCoordinator.Instance.ShowMessageAsync(this, "Save file failed", ex.Message);
                        return;
                    }
                    CreateNewDoc();
                }
                else if (ret == MessageDialogResult.Negative)
                    CreateNewDoc();
            }
            else
                CreateNewDoc();
        });

        private async Task<string> Open(string path)
        {
            try
            {
                var enc = SimpleHelpers.FileEncoding.DetectFileEncoding(path, System.Text.Encoding.UTF8);
                StreamReader sr = new StreamReader(path, enc);
                var content = await sr.ReadToEndAsync();
                sr.Close();
                return content;
            }
            catch(Exception ex)
            {
                await DialogCoordinator.Instance.ShowMessageAsync(this, "Error", $"Open file \"{path}\" failed!\nMessage: {ex.Message}");
                return null;
            }            
        }

        public ICommand OpenDocumentCommand => new RelayCommand(async () =>
        {
            Action OpenDoc = async () =>
            {
                var dlg = new OpenFileDialog();
                dlg.Title = "Open";
                dlg.Filter = Properties.Resources.MarkDownFileFilter;
                if (dlg.ShowDialog() == true)
                {
                    string content = await Open(dlg.FileName);
                    if (content == null)
                        return;

                    SourceCode = new TextDocument(content);
                    SourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => UpdatePreview());
                    UpdatePreview();
                    SourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => IsModified = CanUndo);
                    DocumentPath = dlg.FileName;
                    DocumentTitle = Path.GetFileName(dlg.FileName);
                    IsModified = false;
                    StatusBarText = $"Document \"{dlg.FileName}\" loaded successfully";
                }
            };

            if (IsModified)
            {
                var ret = await DialogCoordinator.Instance.ShowMessageAsync(this, "Unsaved Changes", "Would you want to save your changes?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                    new MetroDialogSettings() { AffirmativeButtonText = "Save", NegativeButtonText = "Don't save", FirstAuxiliaryButtonText = "Cancle" });
                if (ret == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        if (Save() == false)
                            return;
                    }
                    catch (Exception ex)
                    {
                        await DialogCoordinator.Instance.ShowMessageAsync(this, "Save file failed", ex.Message);
                        return;
                    }
                    OpenDoc();
                }
                else if (ret == MessageDialogResult.Negative)
                    OpenDoc();
            }
            else
                OpenDoc();
        });

        public ICommand SaveDocumentCommand => new RelayCommand(async () =>
        {
            try
            {
                Save();                
            }
            catch (Exception ex)
            {
                await DialogCoordinator.Instance.ShowMessageAsync(this, "Save file failed", ex.Message);
            }
        });

        public ICommand SaveAsDocumentCommand => new RelayCommand(async () =>
        {
            var dlg = new SaveFileDialog();
            dlg.Title = "Save As";
            dlg.Filter = Properties.Resources.MarkDownFileFilter;
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    SaveDoc2File(dlg.FileName);
                }
                catch (Exception ex)
                {
                    await DialogCoordinator.Instance.ShowMessageAsync(this, "Save file failed", ex.Message);
                }
                DocumentPath = dlg.FileName;
                DocumentTitle = Path.GetFileName(dlg.FileName);
                IsModified = false;
            }
        });

        private bool Save()
        {
            if (string.IsNullOrEmpty(DocumentPath))
            {
                var dlg = new SaveFileDialog();
                dlg.Title = "Save";
                dlg.Filter = Properties.Resources.MarkDownFileFilter;
                if (dlg.ShowDialog() == true)
                {
                    SaveDoc2File(dlg.FileName);
                    DocumentPath = dlg.FileName;
                    DocumentTitle = Path.GetFileName(dlg.FileName);
                    IsModified = false;
                }
                else
                    return false;
            }
            else
            {
                SaveDoc2File(DocumentPath);
            }
            return true;
        }

        private async void SaveDoc2File(string path)
        { 
            StreamWriter sw = new StreamWriter(path);
            await sw.WriteAsync(SourceCode.Text);
            sw.Close();
            StatusBarText = $"Document \"{path}\" saved successfully";
        }

        #endregion


        #region Editor Commands

        private bool canUndo;
        public bool CanUndo
        {
            get { return canUndo; }
            set
            {
                if (canUndo == value)
                    return;
                canUndo = value;
                IsModified = canUndo;
                RaisePropertyChanged("CanUndo");
            }
        }

        public ICommand UndoCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            editor.Undo();
        });

        private bool canRedo;
        public bool CanRedo
        {
            get { return canRedo; }
            set
            {
                if (canRedo == value)
                    return;
                canRedo = value;
                RaisePropertyChanged("CanRedo");
            }
        }
        public ICommand RedoCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            editor.Redo();
        });

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

        public ICommand UnorderedListCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            if (editor.SelectionLength == 0)
            {
                SourceCode.Insert(editor.SelectionStart, $"\r\n\r\n- {Properties.Resources.ListItem}\r\n\r\n", AnchorMovementType.BeforeInsertion);
                editor.SelectionStart += 6;
                editor.SelectionLength = Properties.Resources.ListItem.Length;
            }
            else
            {
                if (editor.SelectionLength == 0 || editor.SelectedText.Contains("\n"))//MultiLine
                {
                    var t = editor.SelectedText.Replace("\r\n", " ");
                    int oldStart = editor.SelectionStart;
                    SourceCode.Replace(editor.SelectionStart, editor.SelectionLength, $"\r\n\r\n- {t}\r\n\r\n",OffsetChangeMappingType.RemoveAndInsert);
                    editor.SelectionStart = oldStart + 6;
                    editor.SelectionLength = t.Length;
                }
                else//SingleLine
                {
                    var lineDocument = SourceCode.GetLineByOffset(editor.SelectionStart);
                    int lineStartOffset = lineDocument.Offset;
                    if ((editor.SelectionStart - lineStartOffset) >= 2 &&
                            SourceCode.GetText(editor.SelectionStart - 2, 2) == "- " &&
                            string.IsNullOrWhiteSpace(SourceCode.GetText(lineStartOffset, editor.SelectionStart - 2 - lineStartOffset)))
                        SourceCode.Remove(lineStartOffset, editor.SelectionStart- lineStartOffset);
                    else
                    {
                        SourceCode.Replace(editor.SelectionStart, editor.SelectionLength, $"\r\n\r\n- {editor.SelectedText}\r\n\r\n", OffsetChangeMappingType.RemoveAndInsert);
                    }
                }
            }
        });

        public ICommand OrderedListCommand => new RelayCommand<object>((obj) =>
        {
            Func<string, bool> isNumeric = (string message) =>
            {
                try
                {
                    var result = Convert.ToInt32(message);
                    return true;
                }
                catch
                {
                    return false;
                }
            };

            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            if (editor.SelectionLength == 0)
            {
                SourceCode.Insert(editor.SelectionStart, $"\r\n\r\n1. {Properties.Resources.ListItem}\r\n\r\n", AnchorMovementType.BeforeInsertion);
                editor.SelectionStart += 7;
                editor.SelectionLength = Properties.Resources.ListItem.Length;
            }
            else
            {
                if (editor.SelectionLength == 0 || editor.SelectedText.Contains("\n"))//MultiLine
                {
                    var t = editor.SelectedText.Replace("\r\n", " ");
                    int oldStart = editor.SelectionStart;
                    SourceCode.Replace(editor.SelectionStart, editor.SelectionLength, $"\r\n\r\n1. {t}\r\n\r\n", OffsetChangeMappingType.RemoveAndInsert);
                    editor.SelectionStart = oldStart + 7;
                    editor.SelectionLength = t.Length;
                }
                else//SingleLine
                {          
                    var lineDocument = SourceCode.GetLineByOffset(editor.SelectionStart);
                    int lineStartOffset = lineDocument.Offset;
                    if ((editor.SelectionStart - lineStartOffset) >= 3 &&
                            SourceCode.GetText(editor.SelectionStart - 2, 2) == ". " &&
                            isNumeric(SourceCode.GetText(editor.SelectionStart - 3, 1)) &&
                            string.IsNullOrWhiteSpace(SourceCode.GetText(lineStartOffset, editor.SelectionStart - 3 - lineStartOffset)))
                        SourceCode.Remove(lineStartOffset, editor.SelectionStart - lineStartOffset);
                    else
                    {
                        SourceCode.Replace(editor.SelectionStart, editor.SelectionLength, $"\r\n\r\n1. {editor.SelectedText}\r\n\r\n", OffsetChangeMappingType.RemoveAndInsert);
                    }
                }
            }
        });

        public ICommand TitleCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            if (editor.SelectionLength == 0)
            {
                SourceCode.Insert(editor.SelectionStart, $"\r\n\r\n{Properties.Resources.EmptyTitle}\r\n--\r\n\r\n", AnchorMovementType.BeforeInsertion);
                editor.SelectionStart += 4;
                editor.SelectionLength = Properties.Resources.EmptyTitle.Length;
            }
            else
            {
                int oldStart = editor.SelectionStart;
                var t = SourceCode.GetText(editor.SelectionStart,editor.SelectionLength).Replace("\r\n"," ");
                SourceCode.Replace(editor.SelectionStart, editor.SelectionLength, $"\r\n\r\n{t}\r\n--\r\n\r\n", OffsetChangeMappingType.RemoveAndInsert);
                editor.SelectionStart = oldStart + 4;
                editor.SelectionLength = t.Length;
            }

        });

        public ICommand HyperlinkCommand => new RelayCommand<object>(async (obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            string url = await DialogCoordinator.Instance.ShowInputAsync(this, "Hyperlink", "Please input an URL", new MetroDialogSettings() { DefaultText = "http://example.com/ \"Optional Title\"" });
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

        public ICommand ImageCommand => new RelayCommand<object>(async (obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            string url = await DialogCoordinator.Instance.ShowInputAsync(this, "Image", "Please input an image URL", new MetroDialogSettings() { DefaultText = "http://example.com/graph.jpg \"Optional Title\"" });
            if (url != null)
            {
                if (editor.SelectionLength == 0)
                {
                    var toInsert = $"![{Properties.Resources.EmptyImageDescription}]({url})";
                    sourceCode.Insert(editor.SelectionStart, toInsert, AnchorMovementType.BeforeInsertion);
                    editor.SelectionStart += 2;
                    editor.SelectionLength = Properties.Resources.EmptyImageDescription.Length;
                }
                else
                {
                    var toInsert = $"![{editor.SelectedText}]({url})";
                    sourceCode.Replace(editor.SelectionStart, editor.SelectionLength, toInsert, OffsetChangeMappingType.RemoveAndInsert);
                    editor.SelectionLength = 0;
                }
            }
        });

        public ICommand SeparateLineCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            sourceCode.Replace(editor.SelectionStart, editor.SelectionLength, "\r\n\r\n--------\r\n\r\n");
        });

        public ICommand DateStampCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            sourceCode.Replace(editor.SelectionStart, editor.SelectionLength, DateTime.Now.ToLongDateString());
        });

        public ICommand TimeStampCommand => new RelayCommand<object>((obj) =>
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)obj;
            sourceCode.Replace(editor.SelectionStart, editor.SelectionLength, DateTime.Now.ToString());
        });

        #endregion

        #region Preview
        public ICommand RefreshPreviewCommand => new RelayCommand(() =>
        {
            UpdatePreview();
        });
        public ICommand ExternalBrowserCommand => new RelayCommand(() =>
        {
            Process.Start(previewSourceTempPath);
        });
        #endregion

        private async void LoadDefaultDocument()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length >= 2)
            {
                var content = await Open(args[1]);

                SourceCode = new TextDocument(content);
                SourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => UpdatePreview());
                UpdatePreview();
                SourceCode.TextChanged += new EventHandler((object obj, EventArgs e) => IsModified = CanUndo);
                DocumentPath = args[1];
                DocumentTitle = Path.GetFileName(args[1]);
                IsModified = false;
                StatusBarText = $"Document \"{args[1]}\" loaded successfully";
            }
        }

        public async Task<bool> RequestClosing()
        {
            var ret = await DialogCoordinator.Instance.ShowMessageAsync(this, $"Unsaved Changes", "Do you want to save changes?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                new MetroDialogSettings() { AffirmativeButtonText = "Save", NegativeButtonText = "Don't save", FirstAuxiliaryButtonText = "Cancle" });
            if (ret == MessageDialogResult.Affirmative)
            {
                try
                {
                    return !Save();
                }
                catch (Exception ex)
                {
                    await DialogCoordinator.Instance.ShowMessageAsync(this, "Save file failed", ex.Message);
                    return true;
                }
            }
            else if (ret == MessageDialogResult.Negative)
                return false;
            else
                return true;
        }

        private void UpdatePreview()
        {
            if (IsShowPreview)
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

            //DocumrntStatisticsInfo = $"Words: {Regex.Matches(SourceCode.Text, @"[A-Za-z0-9]+").Count}       Characters: {SourceCode.TextLength}       Lines: {SourceCode.LineCount}";
            DocumrntStatisticsInfo = $"Words: {Regex.Matches(SourceCode.Text, @"[\S]+").Count}       Characters: {SourceCode.TextLength}       Lines: {SourceCode.LineCount}";
        }
    }
}