namespace Loupedeck.LHMMonitorPlugin.Commands
{
    using System;
    using Loupedeck;
    using Loupedeck.LHMMonitorPlugin.Helpers;
    using Loupedeck.LHMMonitorPlugin.Services;

    /// <summary>
    /// CPU 사용률을 5x4 블럭 그래프(총 20개)로 표시합니다.
    /// 5%마다 블럭 1개가 빨강으로 채워집니다.
    /// 라벨 상단, 블럭 하단. 아래 왼쪽부터 채웁니다.
    /// </summary>
    public class CpuLoadBlockCommand : BaseSensorCommand
    {
        private const string SensorId = "/amdcpu/0/load/0";
        private const int Columns = 5;
        private const int Rows = 4;
        private const int TotalBlocks = Columns * Rows;

        private static readonly BitmapColor BaseColor = new BitmapColor(0, 200, 80);
        private static readonly BitmapColor FillColor = new BitmapColor(255, 60, 60);

        public CpuLoadBlockCommand()
            : base("CPU Load (Block)", "CPU Load Block View", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "CPU";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensor = service.GetSensor(SensorId);
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var load = LhmDataService.ParseValue(sensor.Value);
            var filledBlocks = (int)(load / 5.0);
            filledBlocks = Math.Max(0, Math.Min(TotalBlocks, filledBlocks));

            builder.DrawText($"CPU {load:F0}%", 0, 2, builder.Width, 16,
                DisplayHelper.LabelColor, 11);
            DrawBlocks(builder, filledBlocks);
        }

        private void DrawBlocks(BitmapBuilder builder, int filledBlocks)
        {
            const int padX = 6;
            const int gap = 3;
            var blockAreaTop = 20;
            var blockAreaBottom = builder.Height - 4;

            var areaWidth = builder.Width - (padX * 2);
            var areaHeight = blockAreaBottom - blockAreaTop;

            var blockWidth = (areaWidth - (gap * (Columns - 1))) / Columns;
            var blockHeight = (areaHeight - (gap * (Rows - 1))) / Rows;

            for (var i = 0; i < TotalBlocks; i++)
            {
                var col = i % Columns;
                var row = i / Columns;

                var x = padX + col * (blockWidth + gap);
                var y = blockAreaBottom - (row + 1) * blockHeight - row * gap;

                var color = i < filledBlocks ? FillColor : BaseColor;
                builder.FillRectangle(x, y, blockWidth, blockHeight, color);
            }
        }
    }
}
