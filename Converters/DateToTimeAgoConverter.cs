using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Converters
{
    public class DateToTimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                TimeSpan timeDifference = DateTime.Now - dateTime;

                if (timeDifference.TotalDays < 1)
                {
                    int hoursAgo = (int)timeDifference.TotalHours;
                    return hoursAgo > 1 ? $"{hoursAgo} hours ago" : "1 hour ago";
                }
                else if (timeDifference.TotalDays < 30)
                {
                    int daysAgo = (int)timeDifference.TotalDays;
                    return daysAgo > 1 ? $"{daysAgo} days ago" : "1 day ago";
                }
                else if (timeDifference.TotalDays < 365)
                {
                    int monthsAgo = (int)(timeDifference.TotalDays / 30);
                    return monthsAgo > 1 ? $"{monthsAgo} months ago" : "1 month ago";
                }
                else
                {
                    int yearsAgo = (int)(timeDifference.TotalDays / 365);
                    return yearsAgo > 1 ? $"{yearsAgo} years ago" : "1 year ago";
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
