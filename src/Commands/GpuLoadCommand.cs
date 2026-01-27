namespace Loupedeck.LHMMonitorPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LHMMonitorPlugin.Helpers;
    using Loupedeck.LHMMonitorPlugin.Services;

    /// <summary>
    /// GPU 사용률을 터치 버튼에 표시합니다.
    /// SensorId: /gpu-nvidia/0/load/0 (GPU Core %)
    /// </summary>
    public class GpuLoadCommand : BaseSensorCommand
    {
        private const string SensorId = "/gpu-nvidia/0/load/0";

        public GpuLoadCommand()
            : base("GPU Load", "GPU Core Usage", "Hardware Monitor")
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

            var load = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetLoadColor(load);
            DrawValue(builder, $"{load:F0}%", color);
        }
    }
}
