using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarkDownEditor
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
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            SetValue(CaretOffsetProperty, TextArea.Caret.Offset);
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
        /// DependencyProperty for the TextEditorCaretOffset binding. 
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

        #region Selection.
        /// <summary>
        /// DependencyProperty for the TextEditor SelectionLength property. 
        /// </summary>
        public static readonly DependencyProperty SelectionLengthProperty =
             DependencyProperty.Register("SelectionLength", typeof(int), typeof(MvvmTextEditor),
             new PropertyMetadata((obj, args) =>
             {
                 MvvmTextEditor target = (MvvmTextEditor)obj;
                 target.SelectionLength = (int)args.NewValue;
             }));

        /// <summary>
        /// Access to the SelectionLength property.
        /// </summary>
        public new int SelectionLength
        {
            get { return base.SelectionLength; }
            set { SetValue(SelectionLengthProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the TextEditor SelectionStart property. 
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty =
             DependencyProperty.Register("SelectionStart", typeof(int), typeof(MvvmTextEditor),
             new PropertyMetadata((obj, args) =>
             {
                 MvvmTextEditor target = (MvvmTextEditor)obj;
                 target.SelectionStart = (int)args.NewValue;
             }));

        /// <summary>
        /// Access to the SelectionStart property.
        /// </summary>
        public new int SelectionStart
        {
            get { return base.SelectionStart; }
            set { SetValue(SelectionStartProperty, value); }
        }
        #endregion // Selection.


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
            set {  }
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
    }
}
