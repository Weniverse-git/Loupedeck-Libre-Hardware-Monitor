namespace Loupedeck.LHMMonitorPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LHMMonitorPlugin.Helpers;
    using Loupedeck.LHMMonitorPlugin.Services;

    /// <summary>
    /// GPU VRAM 사용량을 터치 버튼에 표시합니다.
    /// SensorId: /gpu-nvidia/0/smalldata/1 (Used), /gpu-nvidia/0/smalldata/2 (Total)
    /// </summary>
    public class GpuVramCommand : BaseSensorCommand
    {
        private const string UsedSensorId = "/gpu-nvidia/0/smalldata/1";
        private const string TotalSensorId = "/gpu-nvidia/0/smalldata/2";

        public GpuVramCommand()
            : base("GPU VRAM", "GPU Memory Usage", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "VRAM";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var usedSensor = service.GetSensor(UsedSensorId);
            var totalSensor = service.GetSensor(TotalSensorId);

            if (usedSensor == null || totalSensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var usedMB = LhmDataService.ParseValue(usedSensor.Value);
            var totalMB = LhmDataService.ParseValue(totalSensor.Value);
            var percent = totalMB > 0 ? (usedMB / totalMB) * 100 : 0;
            var color = DisplayHelper.GetLoadColor(percent);

            var usedGB = usedMB / 1024.0;
            DrawValue(builder, $"{usedGB:F0}GB", color);
        }
    }
}
