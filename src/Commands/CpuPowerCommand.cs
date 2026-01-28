namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// CPU Total Power(W)를 터치 버튼에 표시합니다.
    /// 90W 이상 주황, 140W 이상 빨강.
    /// </summary>
    public class CpuPowerCommand : BaseSensorCommand
    {
        private const string SensorId = "/amdcpu/0/power/0";

        public CpuPowerCommand()
            : base("CPU Power", "CPU Package Power", "Hardware Monitor")
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

            var watts = LhmDataService.ParseValue(sensor.Value);
            BitmapColor color;
            if (watts >= 140)
                color = new BitmapColor(255, 60, 60);
            else if (watts >= 90)
                color = new BitmapColor(255, 180, 0);
            else
                color = new BitmapColor(0, 200, 80);

            DrawValue(builder, $"{watts:F0}W", color);
        }
    }
}
