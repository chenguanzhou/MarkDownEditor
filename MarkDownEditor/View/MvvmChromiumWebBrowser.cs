using CefSharp;
using CefSharp.Wpf;
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
    public class MvvmChromiumWebBrowser:ChromiumWebBrowser, INotifyPropertyChanged
    {
        public MvvmChromiumWebBrowser()
        {
            this.RequestHandler = new RequestHandler();
            this.IsBrowserInitializedChanged += MvvmChromiumWebBrowser_IsBrowserInitializedChanged;
                  
        }

        #region IsBrowserInitialized
        private void MvvmChromiumWebBrowser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetValue(IsBrowserInitializedProperty, e.NewValue);
        }

        /// <summary>
        /// DependencyProperty for the ChromiumWebBrowser IsBrowserInitialized property. 
        /// </summary>
        public static readonly new DependencyProperty IsBrowserInitializedProperty =
             DependencyProperty.Register("IsBrowserInitialized", typeof(bool), typeof(MvvmChromiumWebBrowser));

        /// <summary>
        /// Access to the CanUndo property.
        /// </summary>
        public new bool IsBrowserInitialized
        {
            get { return base.IsBrowserInitialized; }
            set { }
        }
        #endregion //IsBrowserInitialized

        /// <summary>
        /// DependencyProperty for the ShouldReload binding. 
        /// </summary>
        public static DependencyProperty ShouldReloadProperty =
            DependencyProperty.Register("ShouldReload", typeof(bool), typeof(MvvmChromiumWebBrowser),
            new PropertyMetadata(default(bool), (obj, args) =>
            {
                MvvmChromiumWebBrowser target = (MvvmChromiumWebBrowser)obj;
                target.ShouldReload = (bool)args.NewValue;

                if (target.IsBrowserInitialized)
                {
                    target.Reload();
                    string src = $"scrollTo(0, {target.ScrollOffsetRatio} * (document.body.offsetHeight - window.innerHeight))";
                    target.GetMainFrame().ExecuteJavaScriptAsync(src);
                }                
            }));

        /// <summary>
        /// Provide access to the ScrollOffsetRatio.
        /// </summary>
        public bool ShouldReload
        {
            get { return (bool)GetValue(ShouldReloadProperty); }
            set { SetValue(ShouldReloadProperty, value); }
        }


        /// <summary>
        /// DependencyProperty for the ScrollOffsetRatio binding. 
        /// </summary>
        public static DependencyProperty ScrollOffsetRatioProperty =
            DependencyProperty.Register("ScrollOffsetRatio", typeof(double), typeof(MvvmChromiumWebBrowser),
            new PropertyMetadata(default(double),(obj, args) =>
            {
                MvvmChromiumWebBrowser target = (MvvmChromiumWebBrowser)obj;
                target.ScrollOffsetRatio = (double)args.NewValue;
                string src = $"scrollTo(0, {target.ScrollOffsetRatio} * (document.body.offsetHeight - window.innerHeight))";
                target.GetMainFrame().ExecuteJavaScriptAsync(src);
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
        
        //support touch
        public bool IsTouched = false;
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (!IsTouched)
            {
                base.OnPreviewMouseDown(e);
            }
            else
            {
                e.Handled = true;
            }
        }
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (!IsTouched)
            {
                base.OnPreviewMouseUp(e);
            }
            else
            {
                e.Handled = true;
            }
            IsTouched = false;
        }
    }
}
