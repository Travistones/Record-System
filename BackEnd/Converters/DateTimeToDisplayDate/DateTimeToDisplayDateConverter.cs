using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Converters.DateTimeToDisplayDate
{
    public class DateTimeToDisplayDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
                return String.Empty;

            DateTime dateTime = DateTime.Now;

            if (value is DateTime)
                dateTime = (DateTime)value;
            else if (value is DateTimeOffset)
                dateTime = ((DateTimeOffset)value).Date;

            if (dateTime.ToString("MMMM dd, yyyy") == DateTime.Now.ToString("MMMM dd, yyyy"))
                return "Today";
            else if (dateTime.ToString("MMMM dd, yyyy") == DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)).ToString("MMMM dd, yyyy"))
                return "Yesterday";
            else if (dateTime.ToString("MMMM dd, yyyy") == DateTime.Now.Add(new TimeSpan(1, 0, 0, 0)).ToString("MMMM dd, yyyy"))
                return "Tomorrow";
            else if (dateTime.ToString("MMMM dd, yyyy") == DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0)).ToString("MMMM dd, yyyy"))
                return "2 Days Ago";
            else if (dateTime.ToString("MMMM dd, yyyy") == DateTime.Now.Add(new TimeSpan(2, 0, 0, 0)).ToString("MMMM dd, yyyy"))
                return "2 Days Later";
            else if (dateTime.ToString("MMMM dd, yyyy") == DateTime.Now.Subtract(new TimeSpan(3, 0, 0, 0)).ToString("MMMM dd, yyyy"))
                return "3 Days Ago";
            else if (dateTime.ToString("MMMM dd, yyyy") == DateTime.Now.Add(new TimeSpan(3, 0, 0, 0)).ToString("MMMM dd, yyyy"))
                return "3 Days Later";
            else if (dateTime.Year == DateTime.Now.Year)
                return dateTime.ToString("MMMM dd");
            else
                return dateTime.ToString("MMMM dd, yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
