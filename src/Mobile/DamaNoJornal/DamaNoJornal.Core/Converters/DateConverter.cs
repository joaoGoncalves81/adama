﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace DamaNoJornal.Core.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime)
            {
                var date = (DateTime)value;

                return date.ToString("dd MMMM", culture).ToUpper();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
