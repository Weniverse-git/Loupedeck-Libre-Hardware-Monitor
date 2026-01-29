namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// GPU 사용률을 터치 버튼에 표시합니다.
    /// 동적 탐지로 NVIDIA, AMD(Radeon), Intel GPU 지원.
    /// </summary>
    public class GpuLoadCommand : BaseSensorCommand
    {
        public GpuLoadCommand()
            : base("GPU Load", "GPU Core Usage", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "GPU";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensorPath = HardwareRegistry.Instance?.GetGpuLoadPath();
            var sensor = sensorPath != null ? service.GetSensor(sensorPath) : null;
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
