namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// GPU 온도를 터치 버튼에 표시합니다.
    /// 동적 탐지로 NVIDIA, AMD(Radeon), Intel GPU 지원.
    /// </summary>
    public class GpuTemperatureCommand : BaseSensorCommand
    {
        public GpuTemperatureCommand()
            : base("GPU Temp", "GPU Core Temperature", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "GPU";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensorPath = HardwareRegistry.Instance?.GetGpuTemperaturePath();
            var sensor = sensorPath != null ? service.GetSensor(sensorPath) : null;
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var temp = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetTemperatureColor(temp, 60, 75);
            DrawValue(builder, $"{temp:F0}\u00b0C", color);
        }
    }
}
