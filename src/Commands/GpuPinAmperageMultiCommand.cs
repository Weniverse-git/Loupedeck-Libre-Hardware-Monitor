namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;
    using System.Collections.Generic;

    /// <summary>
    /// Draws the exact image of Pin Amperage shown in your photo.
    /// Shows all 6 pins on a single button, 3 lines per column.
    /// </summary>
    public class GpuPinAmperageMultiCommand : BaseSensorCommand
    {
        public GpuPinAmperageMultiCommand()
            : base("GPU 12VHPWR All Pins Amperage", "GPU 12VHPWR Amperage Multi-Display", "HW Monitor - GPU 12VHPWR")
        {
        }

        protected override string GetLabel() => "Pins Amperage"; // Optional label for fallback

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            // Define the paths
            var pinIds = new[] {
                "/gpu-nvidia/0/current/1", "/gpu-nvidia/0/current/2", "/gpu-nvidia/0/current/3",
                "/gpu-nvidia/0/current/4", "/gpu-nvidia/0/current/5", "/gpu-nvidia/0/current/6"
            };

            for (int i = 0; i < 6; i++)
            {
                var sensor = service.GetSensor(pinIds[i]);
                double ampValue = sensor != null ? LhmDataService.ParseValue(sensor.Value) : 0;
                
                // Dynamic color selection
                BitmapColor pinColor;
                if (ampValue >= 9.2) pinColor = new BitmapColor(255, 0, 0);      // Red (Dangerous)
                else if (ampValue >= 7.5) pinColor = new BitmapColor(255, 255, 0); // Yellow (Warning)
                else pinColor = new BitmapColor(0, 255, 0);                       // Green (Safe)

                // Calculate position (Left column pins 1-3, Right column pins 4-6)
                int column = i < 3 ? 0 : 1;
                int row = i % 3;
                int x = 0 + (column * 40);
                int y = 5 + (row * 22);

                DrawColumnPin(builder, sensor, i + 1, x, y, pinColor);
            }
        }

        private void DrawColumnPin(BitmapBuilder builder, SensorNode sensor, int pinNumber, int xOffset, int yOffset, BitmapColor color)
        {
            int fontSizeLabel = 10;
            int fontSizeValue = 12;

            // Draw Label ("Pin X") in White
            builder.DrawText($"Pin {pinNumber}", xOffset, yOffset, builder.Width / 2, fontSizeLabel, BitmapColor.White, fontSizeLabel);
            
            // Parse and format the value (e.g., "0.82A")
            string ampText = "N/A";
            if (sensor != null) {
                var amp = LhmDataService.ParseValue(sensor.Value);
                ampText = $"{amp:F2}A"; 
            }

            // Draw Value (e.g., "0.82A") in the dynamic color
            builder.DrawText(ampText, xOffset, yOffset + fontSizeLabel, builder.Width / 2, fontSizeValue, color, fontSizeValue);
        }

        private void DrawOffline(BitmapBuilder builder)
        {
            builder.DrawText("Offline", 0, builder.Height / 2 - 10, builder.Width, 20, BitmapColor.Red, 16);
            builder.DrawText("12VHPWR", 0, builder.Height / 2 + 10, builder.Width, 14, BitmapColor.White, 12);
        }
    }
}