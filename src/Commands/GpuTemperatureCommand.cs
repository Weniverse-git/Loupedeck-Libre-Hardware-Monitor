namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// GPU 온도를 터치 버튼에 표시합니다.
    /// SensorId: /gpu-nvidia/0/temperature/0 (GPU Core)
    /// </summary>
    public class GpuTemperatureCommand : BaseSensorCommand
    {
        private const string SensorId = "/gpu-nvidia/0/temperature/0";

        public GpuTemperatureCommand()
            : base("GPU Temp", "GPU Core Temperature", "Hardware Monitor")
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

            var temp = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetTemperatureColor(temp, 60, 75);
            DrawValue(builder, $"{temp:F0}\u00b0C", color);
        }
    }
}
