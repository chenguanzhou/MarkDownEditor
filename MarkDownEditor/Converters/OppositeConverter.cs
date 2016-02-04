using System;
using System.Globalization;
using System.Windows.Data;

namespace MarkDownEditor.Converters
{
    public class OppositeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => opposite(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => opposite(value);

        private object opposite(object obj)
        {
            if (obj.GetType() == typeof(int))
                return -(int)obj;

            if (obj.GetType() == typeof(double))
                return -(double)obj;

            if (obj.GetType() == typeof(bool))
                return !(bool)obj;

            try
            {
                double val;
                if (Double.TryParse(obj.ToString(),out val) == false)
                    throw new Exception();
                return -val;
            }
            catch (Exception)
            {
                return obj;
            }
        }
    }
}
