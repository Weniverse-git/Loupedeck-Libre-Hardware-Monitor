namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// GPU Package Power(W)를 터치 버튼에 표시합니다.
    /// 동적 탐지로 NVIDIA, AMD(Radeon), Intel GPU 지원.
    /// </summary>
    public class GpuPowerCommand : BaseSensorCommand
    {
        public GpuPowerCommand()
            : base("GPU Power", "GPU Package Power", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "GPU";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensorPath = HardwareRegistry.Instance?.GetGpuPowerPath();
            var sensor = sensorPath != null ? service.GetSensor(sensorPath) : null;
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var watts = LhmDataService.ParseValue(sensor.Value);
            BitmapColor color;
            if (watts >= 500)
                color = new BitmapColor(255, 60, 60);
            else if (watts >= 300)
                color = new BitmapColor(255, 180, 0);
            else
                color = new BitmapColor(0, 200, 80);

            DrawValue(builder, $"{watts:F0}W", color);
        }
    }
}
