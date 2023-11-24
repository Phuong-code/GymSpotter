using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Converters
{
    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int rating)
            {
                //var stars = new List<object>();
                //for (int i = 0; i < rating; i++)
                //{
                //    stars.Add(new object());
                //}
                //return stars;

                return Enumerable.Range(0, rating).Select(_ => "star_icon.png");

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
