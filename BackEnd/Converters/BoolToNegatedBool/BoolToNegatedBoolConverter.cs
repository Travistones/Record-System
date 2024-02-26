using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Converters.BoolToNegatedBool
{
    public class BoolToNegatedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => !(bool)value;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => !(bool)value;
    }
}
