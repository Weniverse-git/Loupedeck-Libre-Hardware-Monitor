namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// RAM (DIMM #3) 온도를 터치 버튼에 표시합니다.
    /// SensorId: /memory/dimm/3/temperature/0
    /// </summary>
    public class RamTemperatureCommand : BaseSensorCommand
    {
        private const string SensorId = "/memory/dimm/3/temperature/0";

        public RamTemperatureCommand()
            : base("RAM Temp", "RAM Temperature", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "RAM";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensor = service.GetSensor(SensorId);
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var temp = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetTemperatureColor(temp, 45, 55);
            DrawValue(builder, $"{temp:F0}\u00b0C", color);
        }
    }
}
