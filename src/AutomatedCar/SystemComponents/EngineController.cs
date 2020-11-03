﻿namespace AutomatedCar.SystemComponents
{
    using SystemComponents.Packets;

    public class EngineController
    {
        private const double GearRatioReverse = 2.9; // Az egyik linkelt pelda atteteibol nezve, konzisztens a tobbi attettel
        private const int RPMDecayPerTick = -10; // Egyelore tetszolegesen eldontott ertek - meg valtozik valoszinuleg
        private const double MinimumBrakeForce = 0.9; 
        private const double MaximumBrakeForce = 0.1;
        private const int ForceToPixelVelocityConversionConstant = 10; // Nagyjabol 10 autohossz/sec-re akartam maximalizalni a sebesseget (~162 km/h 4,5m hosszu autonal), ez a szam azt kozeliti (230px az auto, MaxRPM miatt max ResultantForce 11500)
        private const int MaxRPM = 6000; // Nagyjabol realisztikus maximum, de ez is valtozhat, ha szukseges
        private const int NeutralRPMIncrease = 500; // Egyelore tetszolegesen eldontott ertek - meg valtozik valoszinuleg
        private const int BrakePedalScaling = 10; // Ugy valasztottam, hogy a max fekezesi ero kozelitse a max eloremeneti erot (jelenleg eloremenetMax: 11500, fekMax: 10500). Valtozhat meg
        private const int GasPedalScaling = 1; // Egyelore tetszolegesen eldontott ertek - meg valtozik valoszinuleg

        public int RPM { get; private set; }
        
        public GearShifter GearShifter { get; private set; }

        public double VelocityPixelsPerSecond { get; private set; }

        private IReadOnlyHMIPacket HMIPacket { get; set; }

        public EngineController()
        {
            this.GearShifter = new GearShifter();
            this.RPM = 0;
            this.VelocityPixelsPerSecond = 0;
        }

        public void UpdateEngineProperties(IReadOnlyHMIPacket packet)
        {
            this.HMIPacket = packet;
            this.GearShifter.Position = this.HMIPacket.Gear;
            this.GearShifter.SetDriveGear(this.RPM, this.CalculateRPMChange());
            this.SetRPM();
            this.SetVelocityInPixels();
        }

        private void SetRPM()
        {
            double tempRPM = this.RPM + this.CalculateRPMChange();
            switch (this.GearShifter.Position)
            {
                case Gears.D:
                    tempRPM *= this.AdjustRPMOnGearChange();
                    break;
                case Gears.N:
                    if (this.HMIPacket.Gaspedal != 0)
                    {
                        tempRPM += NeutralRPMIncrease;
                    }
                    break;
                case Gears.R:
                    break;
                case Gears.P:
                    tempRPM = 0;
                    break;
            }
            if ((int)tempRPM < MaxRPM)
            {
                if ((int)tempRPM <= 0) { this.RPM = 0; }
                else { this.RPM = (int)tempRPM; }
            }
            else
            {
                this.RPM = MaxRPM;
            }
        }

        private double AdjustRPMOnGearChange()
        {
            switch (this.GearShifter.DriveGearChangeState)
            {
                case ChangeState.Upshift:
                    return this.GearShifter.CurrentDriveGear.GearRatio / this.GearShifter.DriveGears[this.GearShifter.CurrentDriveGear.SequenceNumber - 1].GearRatio;
                case ChangeState.Downshift:
                    return this.GearShifter.CurrentDriveGear.GearRatio / this.GearShifter.DriveGears[this.GearShifter.CurrentDriveGear.SequenceNumber + 1].GearRatio;
                default:
                    return 1;
            }
        }

        private int CalculateRPMChange() =>
            this.HMIPacket.Gaspedal != 0 ? (int)this.HMIPacket.Gaspedal * GasPedalScaling : RPMDecayPerTick - ((int)this.HMIPacket.Breakpedal * BrakePedalScaling);

        private void SetVelocityInPixels() =>
            this.VelocityPixelsPerSecond = this.CalculateResultantForce() / ForceToPixelVelocityConversionConstant;

        private double CalculateResultantForce() =>
            this.CalculateDriveForce() * this.CalculateBrakeForce();
            //(this.CalculateDriveForce() - this.CalculateBrakeForce() > 0) ? this.CalculateDriveForce() - this.CalculateBrakeForce() : 0;

        private double CalculateBrakeForce() =>
            MinimumBrakeForce - (this.HMIPacket.Breakpedal / (100 / MinimumBrakeForce)) > 0 ? MinimumBrakeForce - (this.HMIPacket.Breakpedal / (100 / MinimumBrakeForce)) : MaximumBrakeForce;

        private double CalculateDriveForce()
        {
            switch (this.GearShifter.Position)
            {
                case Gears.D:
                    return this.RPM / this.GearShifter.CurrentDriveGear.GearRatio;
                case Gears.R:
                    return this.RPM / GearRatioReverse;
                default:
                    return 0;
            }
        }
    }
}
