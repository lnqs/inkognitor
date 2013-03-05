using System;
using System.Diagnostics;
using System.Windows.Data;

namespace Inkognitor
{
    // Taken from http://stackoverflow.com/questions/717299/wpf-setting-the-width-and-height-as-a-percentage-value
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double blah = System.Convert.ToDouble(parameter);
            return System.Convert.ToDouble(value) * (System.Convert.ToDouble(parameter) / 100.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
