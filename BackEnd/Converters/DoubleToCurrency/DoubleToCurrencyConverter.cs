using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;

namespace Record_System.BackEnd.Converters.DoubleToCurrency
{
    public class DoubleToCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (double.IsNaN((double)value)) return "-";

            string amountString = ((double)value).ToString();

            bool isNegative = (((double)value) < 0);

            if (isNegative)
                amountString = amountString.Remove(0, 1);

            string amount = "";

            string decimalPart = "";

            decimal ignoredDecimal;

            if (decimal.TryParse(amountString, out ignoredDecimal))
            {
                bool isDecimalFound = false;

                foreach (var item in amountString)
                {
                    if (item == '.')
                        isDecimalFound = true;

                    if (isDecimalFound)
                        decimalPart += item;
                    else
                        amount += item;
                }
            }
            else amount = amountString;

            if (amount.Count() <= 3) return (isNegative ? "-" : string.Empty) + amount + decimalPart;

            for (int index = amount.Length - 3; index > 0; index -= 3)
            {
                amount = amount.Insert(index, ",");
            }

            return (isNegative ? "-" : string.Empty) + (amount + decimalPart);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
