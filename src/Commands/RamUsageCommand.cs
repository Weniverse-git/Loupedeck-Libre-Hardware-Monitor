namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// RAM 사용률 및 용량을 터치 버튼에 표시합니다.
    /// SensorId: /ram/load/0 (Memory %), /ram/data/0 (Used GB), /ram/data/1 (Available GB)
    /// </summary>
    public class RamUsageCommand : BaseSensorCommand
    {
        private const string LoadSensorId = "/ram/load/0";

        public RamUsageCommand()
            : base("RAM Usage", "Memory Usage", "HW Monitor - RAM")
        {
        }

        protected override string GetLabel() => "RAM";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var loadSensor = service.GetSensor(LoadSensorId);
            if (loadSensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var loadPercent = LhmDataService.ParseValue(loadSensor.Value);
            var color = DisplayHelper.GetLoadColor(loadPercent);
            DrawValue(builder, $"{loadPercent:F0}%", color);
        }
    }
}
