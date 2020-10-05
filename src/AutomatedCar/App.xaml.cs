namespace AutomatedCar
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using AutomatedCar.Models;
    using AutomatedCar.ViewModels;
    using AutomatedCar.Views;
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using Newtonsoft.Json.Linq;

    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"AutomatedCar.Assets.worldobject_polygons.json"));
                string json_text = reader.ReadToEnd();
                dynamic stuff = JObject.Parse(json_text);
                List<Point> points = new List<Point>();
                foreach (var i in stuff["objects"][0]["polys"][0]["points"])
                {
                    points.Add(new Point(i[0].ToObject<int>(), i[1].ToObject<int>()));
                }

                PolylineGeometry geom = new PolylineGeometry(points, false);

                World world = World.Instance;
                world.Width = 500;
                world.Height = 500;

                Circle circle = new Circle(0, 0, "circle.png", 20);
                circle.Width = 40;
                circle.Height = 40;
                circle.ZIndex = 2;
                world.AddObject(circle);


                AutomatedCar controlledCar = new Models.AutomatedCar(0, 0, "car_1_white.png");
                controlledCar.Width = 108;
                controlledCar.Height = 240;

                controlledCar.RadarBrush = new SolidColorBrush(Color.Parse("blue"));

                List<Point> sensorPoints = new List<Point>();
                sensorPoints.Add(new Point(51, 239));
                sensorPoints.Add(new Point(200, 100));
                sensorPoints.Add(new Point(100, 300));

                controlledCar.RadarGeometry = new PolylineGeometry(sensorPoints, false);
                controlledCar.Geometry = geom;
                controlledCar.RadarVisible = true;
                world.AddObject(controlledCar);
                world.ControlledCar = controlledCar;
                controlledCar.Start();

                Game game = new Game(world);
                game.Start();

                desktop.MainWindow = new MainWindow {DataContext = new MainWindowViewModel(world)};
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}