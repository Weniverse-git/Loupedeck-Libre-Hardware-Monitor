namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// CPU 사용량(%), 전력(W), 온도(°C)를 하나의 터치 버튼에 3줄로 통합 표시합니다.
    /// 각 줄은 해당 센서의 임계값에 따라 독립적으로 색상이 변합니다.
    /// </summary>
    public class CpuSummaryCommand : BaseSensorCommand
    {
        public CpuSummaryCommand()
            : base("CPU Summary", "CPU Load / Power / Temperature", "HW Monitor - CPU")
        {
        }

        protected override string GetLabel() => "CPU";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var registry = HardwareRegistry.Instance;
            var loadPath = registry?.GetCpuLoadPath();
            var powerPath = registry?.GetCpuPowerPath();
            var tempPath = registry?.GetCpuTemperaturePath();

            if (loadPath == null && powerPath == null && tempPath == null)
            {
                DrawOfflineState(builder);
                return;
            }

            var loadSensor = loadPath != null ? service.GetSensor(loadPath) : null;
            var powerSensor = powerPath != null ? service.GetSensor(powerPath) : null;
            var tempSensor = tempPath != null ? service.GetSensor(tempPath) : null;

            var load = loadSensor != null ? LhmDataService.ParseValue(loadSensor.Value) : -1;
            var watts = powerSensor != null ? LhmDataService.ParseValue(powerSensor.Value) : -1;
            var temp = tempSensor != null ? LhmDataService.ParseValue(tempSensor.Value) : -1;

            // 라벨
            builder.DrawText("CPU", 0, 2, builder.Width, 16,
                DisplayHelper.LabelColor, 12);

            // 1줄: 사용량 %
            var loadText = load >= 0 ? $"{load:F0}%" : "N/A";
            var loadColor = load >= 0 ? DisplayHelper.GetLoadColor(load) : DisplayHelper.OfflineColor;
            builder.DrawText(loadText, 0, 17, builder.Width, 17,
                loadColor, 14);

            // 2줄: 전력 W
            var powerText = watts >= 0 ? $"{watts:F0}W" : "N/A";
            var powerColor = watts >= 0 ? GetCpuPowerColor(watts) : DisplayHelper.OfflineColor;
            builder.DrawText(powerText, 0, 34, builder.Width, 17,
                powerColor, 14);

            // 3줄: 온도 °C (CPU: 70/90 기준)
            var tempText = temp >= 0 ? $"{temp:F0}\u00B0C" : "N/A";
            var tempColor = temp >= 0 ? DisplayHelper.GetTemperatureColor(temp, 70, 90) : DisplayHelper.OfflineColor;
            builder.DrawText(tempText, 0, 51, builder.Width, 17,
                tempColor, 14);
        }

        protected override void DrawOfflineState(BitmapBuilder builder)
        {
            builder.DrawText("CPU", 0, 2, builder.Width, 16,
                DisplayHelper.LabelColor, 12);
            builder.DrawText("--%", 0, 17, builder.Width, 17,
                DisplayHelper.OfflineColor, 14);
            builder.DrawText("--W", 0, 34, builder.Width, 17,
                DisplayHelper.OfflineColor, 14);
            builder.DrawText("--\u00B0C", 0, 51, builder.Width, 17,
                DisplayHelper.OfflineColor, 14);
        }

        /// <summary>
        /// CPU 전력 색상 (CpuPowerCommand와 동일 기준: 90W/140W)
        /// </summary>
        private static BitmapColor GetCpuPowerColor(double watts)
        {
            if (watts >= 140)
                return new BitmapColor(255, 60, 60);
            if (watts >= 90)
                return new BitmapColor(255, 180, 0);
            return new BitmapColor(0, 200, 80);
        }
    }
}
