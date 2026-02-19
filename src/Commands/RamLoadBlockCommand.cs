namespace Loupedeck.LLHMPlugin.Commands
{
    using System;
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// RAM 사용률을 5x4 블럭 그래프(총 20개)로 표시합니다.
    /// 5%마다 블럭 1개가 빨강으로 채워집니다.
    /// 초기 색상: 진한 노랑. 라벨 상단, 블럭 하단.
    /// </summary>
    public class RamLoadBlockCommand : BaseSensorCommand
    {
        private const string SensorId = "/ram/load/0";
        private const int Columns = 5;
        private const int Rows = 4;
        private const int TotalBlocks = Columns * Rows;

        private static readonly BitmapColor BaseColor = new BitmapColor(200, 170, 0);
        private static readonly BitmapColor FillColor = new BitmapColor(255, 60, 60);

        public RamLoadBlockCommand()
            : base("RAM Load (Block)", "RAM Load Block View", "HW Monitor - RAM")
        {
        }

        protected override string GetLabel() => "RAM";

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

            builder.DrawText($"RAM {load:F0}%", 0, 2, builder.Width, 16,
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

            var bg = DisplayHelper.BackgroundColor;

            for (var i = 0; i < TotalBlocks; i++)
            {
                var col = i % Columns;
                var row = i / Columns;

                var x = padX + col * (blockWidth + gap);
                var y = blockAreaBottom - (row + 1) * blockHeight - row * gap;

                BitmapColor color;
                if (i < filledBlocks)
                {
                    // 채워진 블록: 선명한 노란색
                    color = BaseColor;
                }
                else
                {
                    // 빈 블록: 희미한 노란색 (30% opacity)
                    const double opacity = 0.3;
                    color = new BitmapColor(
                        (byte)(BaseColor.R * opacity + bg.R * (1 - opacity)),
                        (byte)(BaseColor.G * opacity + bg.G * (1 - opacity)),
                        (byte)(BaseColor.B * opacity + bg.B * (1 - opacity))
                    );
                }

                builder.FillRectangle(x, y, blockWidth, blockHeight, color);
            }
        }
    }
}
