using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MarkDownEditor.View
{
    /// <summary>
    /// Class that inherits from the AvalonEdit TextEditor control to 
    /// enable MVVM interaction. 
    /// </summary>
    public class MvvmTextEditor : TextEditor, INotifyPropertyChanged
    {
        /// <summary>
        /// Default constructor to set up event handlers.
        /// </summary>
        public MvvmTextEditor()
        {
            TextArea.SelectionChanged += TextArea_SelectionChanged;
            TextArea.Caret.PositionChanged += Caret_PositionChanged;
            this.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(this.OnScrollChanged));
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            SetValue(CaretOffsetProperty, TextArea.Caret.Offset);
            this.SelectionStart = SelectionStart;
            this.SelectionLength = SelectionLength;
        }

        /// <summary>
        /// Event handler to update properties based upon the selection changed event.
        /// </summary>
        void TextArea_SelectionChanged(object sender, EventArgs e)
        {
            this.SelectionStart = SelectionStart;
            this.SelectionLength = SelectionLength;
        }

        #region Caret Offset.
        /// <summary>
        /// DependencyProperty for the CaretOffset binding. 
        /// </summary>
        public static DependencyProperty CaretOffsetProperty =
            DependencyProperty.Register("CaretOffset", typeof(int), typeof(MvvmTextEditor),
            new PropertyMetadata((obj, args) =>
            {
                MvvmTextEditor target = (MvvmTextEditor)obj;
                target.CaretOffset = (int)args.NewValue;
            }));

        /// <summary>
        /// Provide access to the CaretOffset.
        /// </summary>
        public new int CaretOffset
        {
            get { return base.CaretOffset; }
            set { base.CaretOffset = value; }
            //set { SetValue(CaretOffsetProperty, value); }            
        }
        #endregion // Caret Offset.

        #region Scroll
        protected void OnScrollChanged(object sender, RoutedEventArgs e)
        {
            ScrollViewer viewer = (ScrollViewer)e.OriginalSource;
            ScrollableHeight = viewer.ScrollableHeight;

            ScrollChangedEventArgs args = (ScrollChangedEventArgs)e;
            if (args.VerticalOffset == 0)
            {
                ScrollOffsetRatio = 0;
                return;
            }

            ScrollOffsetRatio = args.VerticalOffset / ScrollableHeight;
        }

        /// <summary>
        /// DependencyProperty for the ScrollOffsetRatio binding. 
        /// </summary>
        public static DependencyProperty ScrollOffsetRatioProperty =
            DependencyProperty.Register("ScrollOffsetRatio", typeof(double), typeof(MvvmTextEditor),
            new PropertyMetadata((obj, args) =>
            {
                MvvmTextEditor target = (MvvmTextEditor)obj;
                target.ScrollOffsetRatio = (double)args.NewValue;
                target.ScrollToVerticalOffset(target.ScrollOffsetRatio * target.ScrollableHeight);
            }));

        /// <summary>
        /// Provide access to the ScrollOffsetRatio.
        /// </summary>
        public double ScrollOffsetRatio
        {
            get { return (double)GetValue(ScrollOffsetRatioProperty); }
            set { SetValue(ScrollOffsetRatioProperty, value); }          
        }

        /// <summary>
        /// Provide access to the ScrollableHeight.
        /// </summary>
        public double ScrollableHeight { get; set; }
        #endregion // Scroll Offset.

        #region Selection.
        /// <summary>
        /// DependencyProperty for the TextEditor SelectionLength property. 
        /// </summary>
        public static readonly DependencyProperty SelectionLengthProperty =
             DependencyProperty.Register("SelectionLength", typeof(int), typeof(MvvmTextEditor),
             new PropertyMetadata((obj, args) =>
             {
                 TextEditor editor = (TextEditor)obj;
                 if (editor.SelectionLength == (int)args.NewValue)
                     return;
                 editor.SelectionLength = (int)args.NewValue;
             }));

        /// <summary>
        /// Access to the SelectionLength property.
        /// </summary>
        public new int SelectionLength
        {
            get { return base.SelectionLength; }
            set
            {
                if ((int)GetValue(SelectionLengthProperty) == value)
                    return;
                SetValue(SelectionLengthProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for the TextEditor SelectionStart property. 
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty =
             DependencyProperty.Register("SelectionStart", typeof(int), typeof(MvvmTextEditor),
             new PropertyMetadata((obj, args) =>
             {
                 TextEditor editor = (TextEditor)obj;
                 if (editor.SelectionStart == (int)args.NewValue)
                     return;
                 ((TextEditor)obj).SelectionStart = (int)args.NewValue;
             }));

        /// <summary>
        /// Access to the SelectionStart property.
        /// </summary>
        public new int SelectionStart
        {
            get { return base.SelectionStart; }
            set
            {
                if ((int)GetValue(SelectionStartProperty) == value)
                    return;
                SetValue(SelectionStartProperty,value);
            }
        }
        #endregion // Selection.

        #region ScrollToSelectionStart

        /// <summary>
        /// DependencyProperty for the ShouldReload binding. 
        /// </summary>
        public static DependencyProperty ScrollToSelectionStartProperty =
            DependencyProperty.Register("ScrollToSelectionStart", typeof(bool), typeof(MvvmTextEditor),
            new PropertyMetadata(default(bool), (obj, args) =>
            {
                MvvmTextEditor editor = (MvvmTextEditor)obj;
                editor.ScrollToSelectionStart = (bool)args.NewValue;

                TextLocation loc = editor.Document.GetLocation(editor.SelectionStart);
                editor.ScrollTo(loc.Line, loc.Column);
            }));

        /// <summary>
        /// Provide access to the ScrollOffsetRatio.
        /// </summary>
        public bool ScrollToSelectionStart
        {
            get { return (bool)GetValue(ScrollToSelectionStartProperty); }
            set { SetValue(ScrollToSelectionStartProperty, value); }
        }

        #endregion //ScrollToSelectionStart


        #region Undo/Redo.
        /// <summary>
        /// Override of OnTextChanged event.
        /// </summary>
        protected override void OnTextChanged(EventArgs e)
        {
            SetValue(CanUndoProperty, CanUndo);
            SetValue(CanRedoProperty, CanRedo);
            base.OnTextChanged(e);
        }

        /// <summary>
        /// DependencyProperty for the TextEditor CanUndo property. 
        /// </summary>
        public static readonly DependencyProperty CanUndoProperty =
             DependencyProperty.Register("CanUndo", typeof(bool), typeof(MvvmTextEditor));

        /// <summary>
        /// Access to the CanUndo property.
        /// </summary>
        public new bool CanUndo
        {
            get { return base.CanUndo; }
            set { }
        }

        /// <summary>
        /// DependencyProperty for the TextEditor CanUndo property. 
        /// </summary>
        public static readonly DependencyProperty CanRedoProperty =
             DependencyProperty.Register("CanRedo", typeof(bool), typeof(MvvmTextEditor));

        /// <summary>
        /// Access to the CanUndo property.
        /// </summary>
        public new bool CanRedo
        {
            get { return base.CanRedo; }
            set { }
        }
        #endregion // Undo/Redo.

        #region EditorOptions

        public static readonly DependencyProperty ShowTabsProperty = DependencyProperty.Register(
             "ShowTabs", typeof(bool), typeof(MvvmTextEditor), new PropertyMetadata(default(bool), ShowTabsChanged));


        private static void ShowTabsChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (MvvmTextEditor)source;
            editor.Options.ShowTabs = editor.ShowTabs;
        }

        public bool ShowTabs
        {
            get { return (bool)GetValue(ShowTabsProperty); }
            set { SetValue(ShowTabsProperty, value); }
        }


        public static readonly DependencyProperty ShowSpacesProperty = DependencyProperty.Register(
            "ShowSpaces", typeof(bool), typeof(MvvmTextEditor), new PropertyMetadata(default(bool), ShowSpacesChanged));

        private static void ShowSpacesChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (MvvmTextEditor)source;
            editor.Options.ShowSpaces = editor.ShowSpaces;
        }

        public bool ShowSpaces
        {
            get { return (bool)GetValue(ShowSpacesProperty); }
            set { SetValue(ShowSpacesProperty, value); }
        }


        public static readonly DependencyProperty ShowEndOfLineProperty = DependencyProperty.Register(
            "ShowEndOfLine", typeof(bool), typeof(MvvmTextEditor), new PropertyMetadata(default(bool), ShowEndOfLineChanged));

        private static void ShowEndOfLineChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (MvvmTextEditor)source;
            editor.Options.ShowEndOfLine = editor.ShowEndOfLine;
        }

        public bool ShowEndOfLine
        {
            get { return (bool)GetValue(ShowEndOfLineProperty); }
            set { SetValue(ShowEndOfLineProperty, value); }
        }


        public static readonly DependencyProperty ShowColumnRulerProperty = DependencyProperty.Register(
            "ShowColumnRuler", typeof(bool), typeof(MvvmTextEditor), new PropertyMetadata(default(bool), ShowColumnRulerChanged));

        private static void ShowColumnRulerChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (MvvmTextEditor)source;
            editor.Options.ShowColumnRuler = editor.ShowColumnRuler;
        }

        public bool ShowColumnRuler
        {
            get { return (bool)GetValue(ShowColumnRulerProperty); }
            set { SetValue(ShowColumnRulerProperty, value); }
        }

        public static readonly DependencyProperty RulerPositionProperty = DependencyProperty.Register(
            "RulerPosition", typeof(int), typeof(MvvmTextEditor), new PropertyMetadata(default(int), RulerPositionChanged));

        private static void RulerPositionChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (MvvmTextEditor)source;
            editor.Options.ColumnRulerPosition = editor.RulerPosition;
        }

        public int RulerPosition
        {
            get { return (int)GetValue(RulerPositionProperty); }
            set { SetValue(RulerPositionProperty, value); }
        }


        public static readonly DependencyProperty HighlightCurrentLineProperty = DependencyProperty.Register(
            "HighlightCurrentLine", typeof(bool), typeof(MvvmTextEditor), new PropertyMetadata(default(bool), HighlightCurrentLineChanged));

        private static void HighlightCurrentLineChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (MvvmTextEditor)source;
            editor.Options.HighlightCurrentLine = editor.HighlightCurrentLine;
        }

        public bool HighlightCurrentLine
        {
            get { return (bool)GetValue(HighlightCurrentLineProperty); }
            set { SetValue(HighlightCurrentLineProperty, value); }
        }

        #endregion

        /// <summary>
        /// Implement the INotifyPropertyChanged event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            string src = $"onmousedown=new Function(\"return true\")";
            this.GetMainFrame().ExecuteJavaScriptAsync(src);
        }
    }
}
