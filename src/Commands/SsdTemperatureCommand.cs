namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    public class SsdTemp0Command : BaseSensorCommand
    {
        private const string SensorId = "/nvme/0/temperature/0";

        public SsdTemp0Command()
            : base("NVMe #0 Temp", "NVMe #0 Temperature", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "NVMe #0";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensor = service.GetSensor(SensorId);
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var temp = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetTemperatureColor(temp, 60, 75);
            DrawValue(builder, $"{temp:F0}\u00b0C", color);
        }
    }

    public class SsdTemp1Command : BaseSensorCommand
    {
        private const string SensorId = "/nvme/1/temperature/0";

        public SsdTemp1Command()
            : base("NVMe #1 Temp", "NVMe #1 Temperature", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "NVMe #1";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensor = service.GetSensor(SensorId);
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var temp = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetTemperatureColor(temp, 60, 75);
            DrawValue(builder, $"{temp:F0}\u00b0C", color);
        }
    }

    public class SsdTemp2Command : BaseSensorCommand
    {
        private const string SensorId = "/nvme/2/temperature/0";

        public SsdTemp2Command()
            : base("NVMe #2 Temp", "NVMe #2 Temperature", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "NVMe #2";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensor = service.GetSensor(SensorId);
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var temp = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetTemperatureColor(temp, 60, 75);
            DrawValue(builder, $"{temp:F0}\u00b0C", color);
        }
    }

    public class SsdTemp3Command : BaseSensorCommand
    {
        private const string SensorId = "/nvme/3/temperature/0";

        public SsdTemp3Command()
            : base("NVMe #3 Temp", "NVMe #3 Temperature", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "NVMe #3";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensor = service.GetSensor(SensorId);
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var temp = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetTemperatureColor(temp, 60, 75);
            DrawValue(builder, $"{temp:F0}\u00b0C", color);
        }
    }

    public class SsdTemp4Command : BaseSensorCommand
    {
        private const string SensorId = "/nvme/4/temperature/0";

        public SsdTemp4Command()
            : base("NVMe #4 Temp", "NVMe #4 Temperature", "Hardware Monitor")
        {
        }

        protected override string GetLabel() => "NVMe #4";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensor = service.GetSensor(SensorId);
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var temp = LhmDataService.ParseValue(sensor.Value);
            var color = DisplayHelper.GetTemperatureColor(temp, 60, 75);
            DrawValue(builder, $"{temp:F0}\u00b0C", color);
        }
    }
}
