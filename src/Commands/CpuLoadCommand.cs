namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// CPU 사용률을 터치 버튼에 표시합니다.
    /// 동적 탐지로 AMD, Intel CPU 지원.
    /// </summary>
    public class CpuLoadCommand : BaseSensorCommand
    {
        public CpuLoadCommand()
            : base("CPU Load", "CPU Total Usage", "HW Monitor - CPU")
        {
        }

        protected override string GetLabel() => "CPU";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensorPath = HardwareRegistry.Instance?.GetCpuLoadPath();
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
