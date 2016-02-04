using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MarkDownEditor.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        private bool isReversed;
#if !SILVERLIGHT
        private bool useHidden;
#endif

        /// <summary>
        /// Initializes a new instance of the BooleanToVisibilityConverter class.
        /// </summary>
        public BooleanToVisibilityConverter()
        {
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the BooleanToVisibilityConverter class.
        /// </summary>
        /// <param name="isReversed">
        /// Whether the return values should be reversed.
        /// </param>
        /// <param name="useHidden">
        /// Whether <see cref="Visibility.Hidden"/> should be used instead of <see cref="Visibility.Collapsed"/>.
        /// </param>
        public BooleanToVisibilityConverter(bool isReversed, bool useHidden)
        {
            this.isReversed = isReversed;
            this.useHidden = useHidden;
        }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether the return values should be reversed.
        /// </summary>
        public bool IsReversed
        {
            get { return this.isReversed; }
            set { this.isReversed = value; }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Visibility.Hidden"/> should be returned instead of <see cref="Visibility.Collapsed"/>.
        /// </summary>
        public bool UseHidden
        {
            get { return this.useHidden; }
            set { this.useHidden = value; }
        }
#endif

        /// <summary>
        /// Attempts to convert the specified value.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <param name="targetType">
        /// The type of the binding target property.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
        /// <returns>
        /// A converted value.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);

            if (this.IsReversed)
            {
                val = !val;
            }

            if (val)
            {
                return Visibility.Visible;
            }

#if !SILVERLIGHT
            return this.UseHidden ? Visibility.Hidden : Visibility.Collapsed;
#else
            return Visibility.Collapsed;
#endif
        }

        /// <summary>
        /// Attempts to convert the specified value back.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <param name="targetType">
        /// The type of the binding target property.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
        /// <returns>
        /// A converted value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
            {
                return DependencyProperty.UnsetValue;
            }

            var visibility = (Visibility)value;
            var result = visibility == Visibility.Visible;

            if (this.IsReversed)
            {
                result = !result;
            }

            return result;
        }
    }
}
