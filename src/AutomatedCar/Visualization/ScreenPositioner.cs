namespace AutomatedCar
{
    using System;
    using AutomatedCar.Models;
    using AutomatedCar.Visualization;
    using Avalonia.Input;
    using Avalonia;

    public class ScreenPositioner 
    {

        public static ScreenPositioner Instance { get; } = new ScreenPositioner();

        public void AlignItemsToScreen(World world) {
            
            PositionComputeObject pco = new PositionComputeObject(world.ControlledCar);

            foreach(WorldObject wo in world.WorldObjects) {

                Point p = pco.getPositionFromScreen(wo, world.VisibleWidth, world.VisibleHeight);
                if(this.closeToLeftEdge(world, world.ControlledCar)) wo.VisibleX = wo.X;
                else if(this.closeToRightEdge(world, world.ControlledCar)) wo.VisibleX = world.VisibleWidth - wo.Width;
                else wo.VisibleX = Convert.ToInt32(p.X);
                if(this.closeToTopEdge(world, world.ControlledCar)) wo.VisibleY = wo.Y;
                else if(this.closeToBottomEdge(world, world.ControlledCar)) wo.VisibleY = world.VisibleHeight - wo.Height;
                else wo.VisibleY = Convert.ToInt32(p.Y);
                
            }


            if(this.closeToLeftEdge(world, world.ControlledCar)) world.ControlledCar.VisibleX = computeX(world, world.ControlledCar.X);
            else if(this.closeToRightEdge(world, world.ControlledCar)) world.ControlledCar.VisibleX = world.VisibleWidth - (world.Width - world.ControlledCar.X)-world.ControlledCar._rotationCenterPointX - world.ControlledCar.Width/2;
            else world.ControlledCar.VisibleX = computeX(world, world.VisibleWidth/2);

            if(this.closeToTopEdge(world, world.ControlledCar)) world.ControlledCar.VisibleY = computeY(world, world.ControlledCar.Y);
            else if(this.closeToBottomEdge(world, world.ControlledCar)) world.ControlledCar.VisibleY = world.VisibleHeight - (world.Height - world.ControlledCar.Y)-world.ControlledCar._rotationCenterPointY;
            else world.ControlledCar.VisibleY = computeY(world, world.VisibleHeight/2);
        }
 
        private int computeX(World world, int currentX) {
            return currentX-world.ControlledCar._rotationCenterPointX - world.ControlledCar.Width/2;
        }
        private int computeY(World world, int currentY) {
            return currentY-world.ControlledCar._rotationCenterPointY;
        }

        private bool closeToLeftEdge(World world, AutomatedCar car){
            return  car.X < (world.VisibleWidth/2);
        }

        private bool closeToTopEdge(World world, AutomatedCar car){
            return  car.Y < (world.VisibleHeight/2);
        }

         private bool closeToRightEdge(World world, AutomatedCar car){
            return  car.X > (world.Width - (world.VisibleWidth/2));
        }

        private bool closeToBottomEdge(World world, AutomatedCar car){
            return  car.Y > (world.Height - (world.VisibleHeight/2));
        }

    }
}