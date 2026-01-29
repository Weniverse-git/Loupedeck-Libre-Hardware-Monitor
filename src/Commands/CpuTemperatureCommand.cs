namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// CPU 온도를 터치 버튼에 표시합니다.
    /// 동적 탐지로 AMD, Intel CPU 지원.
    /// </summary>
    public class CpuTemperatureCommand : BaseSensorCommand
    {
        public CpuTemperatureCommand()
            : base("CPU Temp", "CPU Core Temperature", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "CPU";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensorPath = HardwareRegistry.Instance?.GetCpuTemperaturePath();
            var sensor = sensorPath != null ? service.GetSensor(sensorPath) : null;
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var temp = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetTemperatureColor(temp, 70, 90);
            DrawValue(builder, $"{temp:F0}\u00b0C", color);
        }
    }
}
