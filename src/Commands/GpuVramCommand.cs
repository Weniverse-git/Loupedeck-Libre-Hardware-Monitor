namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// GPU VRAM 사용량을 터치 버튼에 표시합니다.
    /// 동적 탐지로 NVIDIA, AMD(Radeon), Intel GPU 지원.
    /// </summary>
    public class GpuVramCommand : BaseSensorCommand
    {
        public GpuVramCommand()
            : base("GPU VRAM", "GPU Memory Usage", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "VRAM";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var usedPath = HardwareRegistry.Instance?.GetGpuVramUsedPath();
            var totalPath = HardwareRegistry.Instance?.GetGpuVramTotalPath();
            var usedSensor = usedPath != null ? service.GetSensor(usedPath) : null;
            var totalSensor = totalPath != null ? service.GetSensor(totalPath) : null;

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
