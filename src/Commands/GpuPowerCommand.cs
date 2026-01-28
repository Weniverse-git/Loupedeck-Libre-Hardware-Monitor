namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// GPU Package Power(W)를 터치 버튼에 표시합니다.
    /// 300W 이상 주황, 500W 이상 빨강.
    /// </summary>
    public class GpuPowerCommand : BaseSensorCommand
    {
        private const string SensorId = "/gpu-nvidia/0/power/0";

        public GpuPowerCommand()
            : base("GPU Power", "GPU Package Power", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "GPU";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensor = service.GetSensor(SensorId);
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
