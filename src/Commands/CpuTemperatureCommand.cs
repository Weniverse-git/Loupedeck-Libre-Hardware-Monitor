namespace Loupedeck.LHMMonitorPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LHMMonitorPlugin.Helpers;
    using Loupedeck.LHMMonitorPlugin.Services;

    /// <summary>
    /// CPU 온도를 터치 버튼에 표시합니다.
    /// SensorId: /amdcpu/0/temperature/2 (Core Tctl/Tdie)
    /// </summary>
    public class CpuTemperatureCommand : BaseSensorCommand
    {
        private const string SensorId = "/amdcpu/0/temperature/2";

        public CpuTemperatureCommand()
            : base("CPU Temp", "CPU Core Temperature", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "CPU";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensor = service.GetSensor(SensorId);
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var temp = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetTemperatureColor(temp);
            DrawValue(builder, $"{temp:F0}\u00b0C", color);
        }
    }
}
