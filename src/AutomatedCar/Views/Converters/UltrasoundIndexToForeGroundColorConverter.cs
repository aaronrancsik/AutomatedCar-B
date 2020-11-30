namespace AutomatedCar.Views.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents;
    using Avalonia;
    using Avalonia.Media;

    public class UltrasoundIndexToForeGroundColorConverter : UltrasoundConvert
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Ultrasound[] ultras = World.Instance.ControlledCar.Ultrasounds;
            if (ultras.Where(x => x.Points == (List<Point>)value).FirstOrDefault() != null)
            {
                double distance = ultras.Where(x => x.Points == (List<Point>)value).FirstOrDefault().Distance;
                return this.ColorSwitcher(distance, "#EA0B0B", "#FFA500", "#1E8449");
            }
            else
            {
                return Brush.Parse("#85929E");
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
