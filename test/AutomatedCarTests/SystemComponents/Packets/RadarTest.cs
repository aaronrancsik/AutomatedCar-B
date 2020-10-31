using AutomatedCar.SystemComponents;
using AutomatedCar.SystemComponents.Packets;
using Avalonia.Layout;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using System.ComponentModel;
using Xunit;
using AutomatedCar.Models;
using System.Collections.Generic;
using Avalonia.Media;


namespace Tests.SystemComponents.Packets
{
    public class RadarTest
    {
        Radar radar = new Radar();

    // Radar
        // szög
        int FOV = 60;
        //forgás

        int _angle = 0;
        int Angle {
            get => this._angle;
            set => _angle = value;
        }
        // Táv (szögtől a túlsó élig)
        int Distance = 200*50;
        // Autó középpontjához képesti függőleges eltolás
        int Offset = 120;
    //Autó
        int CarWidth = 20;
        int CarHeight = 40;
        int CarRotation = 0;
        int CarX = 1;
        int CarY = 1;

        [Fact]
        public void Radar_FromOrigo_rotate180_distance0_offset0()
        {

            this.CarRotation = 180;
            this.CarX = 0;
            this.CarY = 0;
            this.Distance = 0;
            this.Offset = 0;

            var points = this.radar.computeTriangleInWorld();

            Assert.Equal(0,points[0].X);
            Assert.Equal(0,points[0].Y);
            Assert.Equal(0,points[1].X);
            Assert.Equal(0,points[1].Y);
            Assert.Equal(0,points[2].X);
            Assert.Equal(0,points[2].Y);
        }

        [Fact]
        public void Radar_FromOrigo_rotate180_distance0_offset120()
        {
            this.CarRotation = 180;
            this.CarX = 0;
            this.CarY = 0;
            this.Distance = 0;
            this.Offset = 120;


            this.radar.offset = this.Offset;

            var points = this.radar.computeTriangleInWorld(); 

            Assert.Equal(0, points[0].X);
            Assert.Equal(120, points[0].Y);
            Assert.Equal(0, points[1].X);
            Assert.Equal(120, points[1].Y);
            Assert.Equal(0, points[2].X);
            Assert.Equal(120, points[2].Y);
        }

        
    }
}