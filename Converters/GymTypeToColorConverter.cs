using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Converters
{
    public class GymTypeToColorConverter : IValueConverter
    {
        private static readonly Dictionary<string, Color> SpecificColors = new Dictionary<string, Color>
        {
            { "24/7", Color.FromRgba(229, 204, 253, 255) },             // Light Purple
            { "Dojo", Color.FromRgba(254, 233, 168, 255) },             // Ligh Yellow
            { "MMA", Color.FromRgba(179, 179, 179, 255) },              // Light Grey
            { "Boxing", Color.FromRgba(190, 226, 253, 255) },           // Light Blue
            { "Crossfit", Color.FromRgba(174, 243, 199, 255) },         // Light Green
            { "Powerlifting", Color.FromRgba(255, 201, 195, 255) },     // Light Red
            { "Female Only", Color.FromRgba(252, 182, 242, 255) },      // Light Pink 
            { "Rock Climbing", Color.FromRgba(251, 211, 160, 255) }     // Light Brown
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string gymType)
            {
                if (SpecificColors.ContainsKey(gymType))
                {
                    return SpecificColors[gymType];
                }
            }

            return Color.FromRgba(195, 187, 187, 255);  //Light Grey
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
