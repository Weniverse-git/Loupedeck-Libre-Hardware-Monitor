namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;
    using System;

    public class GpuPinAmperageMultiCommand : BaseSensorCommand
    {
        // Constants for better maintainability and "clean code"
        private const Double WarningThreshold = 7.5;
        private const Double CriticalThreshold = 9.2;
        private const Int32 TotalPins = 6;

        public GpuPinAmperageMultiCommand()
            : base("GPU 12VHPWR All Pins Amperage", "GPU 12VHPWR Amperage Multi-Display", "HW Monitor - GPU 12VHPWR")
        {
        }

        protected override String GetLabel() => "Pins Amperage";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            for (var i = 0; i < TotalPins; i++)
            {
                // GPU sensor paths usually start at 1
                var pinIndex = i + 1;
                var sensorPath = $"/gpu-nvidia/0/current/{pinIndex}";
                var sensor = service.GetSensor(sensorPath);
                
                // Optimization: Parse once here and pass the value down
                var ampValue = sensor != null ? LhmDataService.ParseValue(sensor.Value) : 0.0;
                
                var pinColor = this.GetThresholdColor(ampValue);

                // Calculate grid position
                var isRightColumn = i >= 3;
                var column = isRightColumn ? 1 : 0;
                var row = i % 3;

                var x = column * 40;
                var y = 5 + (row * 22);

                this.DrawPinCell(builder, pinIndex, ampValue, x, y, pinColor);
            }
        }

        private BitmapColor GetThresholdColor(Double value)
        {
            if (value >= CriticalThreshold) { return new BitmapColor(255, 0, 0); }    // Red
            if (value >= WarningThreshold) { return new BitmapColor(255, 255, 0); }  // Yellow
            return new BitmapColor(0, 255, 0);                                      // Green
        }

        private void DrawPinCell(BitmapBuilder builder, Int32 pinNumber, Double value, Int32 x, Int32 y, BitmapColor color)
        {
            const Int32 FontSizeLabel = 10;
            const Int32 FontSizeValue = 12;

            // Draw Label
            builder.DrawText($"Pin {pinNumber}", x, y, builder.Width / 2, FontSizeLabel, BitmapColor.White, FontSizeLabel);
            
            // Draw Value
            var text = $"{value:F2}A";
            builder.DrawText(text, x, y + FontSizeLabel, builder.Width / 2, FontSizeValue, color, FontSizeValue);
        }

        private void DrawOffline(BitmapBuilder builder)
        {
            builder.DrawText("Offline", 0, builder.Height / 2 - 10, builder.Width, 20, BitmapColor.Red, 16);
            builder.DrawText("12VHPWR", 0, builder.Height / 2 + 10, builder.Width, 14, BitmapColor.White, 12);
        }
    }
}