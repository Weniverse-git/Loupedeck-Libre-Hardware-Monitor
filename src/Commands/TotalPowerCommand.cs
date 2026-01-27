namespace Loupedeck.LHMMonitorPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LHMMonitorPlugin.Helpers;
    using Loupedeck.LHMMonitorPlugin.Services;

    /// <summary>
    /// CPU + GPU 합산 Power(W)를 터치 버튼에 표시합니다.
    /// 440W 이상 주황, 540W 이상 빨강.
    /// </summary>
    public class TotalPowerCommand : BaseSensorCommand
    {
        private const string CpuPowerSensorId = "/amdcpu/0/power/0";
        private const string GpuPowerSensorId = "/gpu-nvidia/0/power/0";

        public TotalPowerCommand()
            : base("CPGPU Power", "CPU + GPU Total Power", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "CPGPU";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var cpuSensor = service.GetSensor(CpuPowerSensorId);
            var gpuSensor = service.GetSensor(GpuPowerSensorId);

            if (cpuSensor == null && gpuSensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var cpuWatts = cpuSensor != null ? LhmDataService.ParseValue(cpuSensor.Value) : 0;
            var gpuWatts = gpuSensor != null ? LhmDataService.ParseValue(gpuSensor.Value) : 0;
            var totalWatts = cpuWatts + gpuWatts;

            BitmapColor color;
            if (totalWatts >= 540)
                color = new BitmapColor(255, 60, 60);
            else if (totalWatts >= 440)
                color = new BitmapColor(255, 180, 0);
            else
                color = new BitmapColor(0, 200, 80);

            DrawValue(builder, $"{totalWatts:F0}W", color);
        }
    }
}
