namespace AutomatedCar.Models
{
    using ReactiveUI;

    public abstract class WorldObject : ReactiveObject
    {
        private int _x;
        private int _y;
        private double _angle = 90; // mocking
        public int _rotationCenterPointX = 90; // = width/2
        public int _rotationCenterPointY = 120; // = height/2

        public WorldObject(int x, int y, string filename)
        {
            this.X = x;
            this.Y = y;
            this.Filename = filename;
            this.ZIndex = 1;
        }

        public int ZIndex { get; set; }
        public double Angle
        {
            get => this._angle;
        }

        public int RotationCenterPointX
        {
            get => this._rotationCenterPointX;
        }
        public int RotationCenterPointY
        {
            get => this._rotationCenterPointY;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public int X
        {
            get => this._x;
            set => this.RaiseAndSetIfChanged(ref this._x, value);
        }

        public int Y
        {
            get => this._y;
            set => this.RaiseAndSetIfChanged(ref this._y, value);
        }

        public string Filename { get; set; }
        public int Offset { get => 50; }
    }
}