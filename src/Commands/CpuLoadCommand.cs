namespace Loupedeck.LHMMonitorPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LHMMonitorPlugin.Helpers;
    using Loupedeck.LHMMonitorPlugin.Services;

    /// <summary>
    /// CPU 사용률을 터치 버튼에 표시합니다.
    /// SensorId: /amdcpu/0/load/0 (CPU Total %)
    /// </summary>
    public class CpuLoadCommand : BaseSensorCommand
    {
        private const string SensorId = "/amdcpu/0/load/0";

        public CpuLoadCommand()
            : base("CPU Load", "CPU Total Usage", "Hardware Monitor")
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

            var load = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetLoadColor(load);
            DrawValue(builder, $"{load:F0}%", color);
        }
    }
}
