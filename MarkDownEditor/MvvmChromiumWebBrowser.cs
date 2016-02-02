using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarkDownEditor
{
    public class MvvmChromiumWebBrowser:ChromiumWebBrowser, INotifyPropertyChanged
    {
        public MvvmChromiumWebBrowser()
        {
            this.IsBrowserInitializedChanged += MvvmChromiumWebBrowser_IsBrowserInitializedChanged;
        }

        private void MvvmChromiumWebBrowser_IsBrowserInitializedChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            SetValue(IsBrowserInitializedProperty, e.NewValue);
        }

        /// <summary>
        /// DependencyProperty for the ChromiumWebBrowser IsBrowserInitialized property. 
        /// </summary>
        public static readonly DependencyProperty IsBrowserInitializedProperty =
             DependencyProperty.Register("IsBrowserInitialized", typeof(bool), typeof(MvvmTextEditor));

        /// <summary>
        /// Access to the CanUndo property.
        /// </summary>
        public new bool IsBrowserInitialized
        {
            get { return base.IsBrowserInitialized; }
            set { }
        }


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
