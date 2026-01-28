namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    public class GpuMemoryLoadCommand : BaseSensorCommand
    {
        private const string UsedSensorId = "/gpu-nvidia/0/smalldata/1";
        private const string TotalSensorId = "/gpu-nvidia/0/smalldata/2";

        public GpuMemoryLoadCommand()
            : base("GPU VRAM %", "GPU Memory Usage Percent", "Hardware Monitor")
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
            DrawValue(builder, $"{percent:F0}%", color);
        }
    }
}
