// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MsiFinder.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !Equals(value, true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !Equals(value, true);
        }
    }
}
