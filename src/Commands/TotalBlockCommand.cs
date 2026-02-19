namespace Loupedeck.LLHMPlugin.Commands
{
    using System;
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// CPU/GPU/VRAM/RAM 4줄 통합 블럭 그래프.
    /// 각 줄 5블럭, 블럭 하나당 20%.
    /// 동적 탐지로 AMD/Intel CPU, NVIDIA/AMD/Intel GPU 지원.
    /// </summary>
    public class TotalBlockCommand : BaseSensorCommand
    {
        private const string RamLoadSensorId = "/ram/load/0";
        private const int Columns = 5;
        private const int RowCount = 4;

        private static readonly BitmapColor CpuBaseColor = new BitmapColor(150, 150, 150);
        private static readonly BitmapColor GpuBaseColor = new BitmapColor(60, 120, 255);
        private static readonly BitmapColor VramBaseColor = new BitmapColor(180, 100, 255);
        private static readonly BitmapColor RamBaseColor = new BitmapColor(200, 170, 0);
        private static readonly BitmapColor FillColor = new BitmapColor(255, 60, 60);

        public TotalBlockCommand()
            : base("Total Load (Block)", "TOTAL Load Block View", "HW Monitor - ETC")
        {
        }

        protected override string GetLabel() => "TOTAL";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var cpuLoad = GetCpuLoadPercent(service);
            var gpuLoad = GetGpuLoadPercent(service);
            var vramLoad = GetVramPercent(service);
            var ramLoad = GetRamLoadPercent(service);

            builder.DrawText(GetLabel(), 0, 2, builder.Width, 16,
                DisplayHelper.LabelColor, 11);

            DrawRow(builder, 0, cpuLoad, CpuBaseColor);
            DrawRow(builder, 1, gpuLoad, GpuBaseColor);
            DrawRow(builder, 2, vramLoad, VramBaseColor);
            DrawRow(builder, 3, ramLoad, RamBaseColor);
        }

        private double GetCpuLoadPercent(LhmDataService service)
        {
            var sensorPath = HardwareRegistry.Instance?.GetCpuLoadPath();
            var sensor = sensorPath != null ? service.GetSensor(sensorPath) : null;
            return sensor != null ? LhmDataService.ParseValue(sensor.Value) : 0;
        }

        private double GetGpuLoadPercent(LhmDataService service)
        {
            var sensorPath = HardwareRegistry.Instance?.GetGpuLoadPath();
            var sensor = sensorPath != null ? service.GetSensor(sensorPath) : null;
            return sensor != null ? LhmDataService.ParseValue(sensor.Value) : 0;
        }

        private double GetVramPercent(LhmDataService service)
        {
            var usedPath = HardwareRegistry.Instance?.GetGpuVramUsedPath();
            var totalPath = HardwareRegistry.Instance?.GetGpuVramTotalPath();
            var used = usedPath != null ? service.GetSensor(usedPath) : null;
            var total = totalPath != null ? service.GetSensor(totalPath) : null;
            if (used == null || total == null) return 0;
            var usedMB = LhmDataService.ParseValue(used.Value);
            var totalMB = LhmDataService.ParseValue(total.Value);
            return totalMB > 0 ? (usedMB / totalMB) * 100 : 0;
        }

        private double GetRamLoadPercent(LhmDataService service)
        {
            var sensor = service.GetSensor(RamLoadSensorId);
            return sensor != null ? LhmDataService.ParseValue(sensor.Value) : 0;
        }

        private void DrawRow(BitmapBuilder builder, int rowIndex, double percent, BitmapColor baseColor)
        {
            const int padX = 6;
            const int gap = 3;
            var blockAreaTop = 20;
            var blockAreaBottom = builder.Height - 4;

            var areaWidth = builder.Width - (padX * 2);
            var areaHeight = blockAreaBottom - blockAreaTop;

            var blockWidth = (areaWidth - (gap * (Columns - 1))) / Columns;
            var blockHeight = (areaHeight - (gap * (RowCount - 1))) / RowCount;

            var filled = (int)(percent / 20.0);
            filled = Math.Max(0, Math.Min(Columns, filled));

            var bg = DisplayHelper.BackgroundColor;

            for (var col = 0; col < Columns; col++)
            {
                var x = padX + col * (blockWidth + gap);
                var y = blockAreaTop + rowIndex * (blockHeight + gap);

                BitmapColor color;
                if (col < filled)
                {
                    // 채워진 블록: 선명한 색상
                    color = baseColor;
                }
                else
                {
                    // 빈 블록: 희미한 색상 (30% opacity)
                    const double opacity = 0.3;
                    color = new BitmapColor(
                        (byte)(baseColor.R * opacity + bg.R * (1 - opacity)),
                        (byte)(baseColor.G * opacity + bg.G * (1 - opacity)),
                        (byte)(baseColor.B * opacity + bg.B * (1 - opacity))
                    );
                }

                builder.FillRectangle(x, y, blockWidth, blockHeight, color);
            }
        }
    }
}
