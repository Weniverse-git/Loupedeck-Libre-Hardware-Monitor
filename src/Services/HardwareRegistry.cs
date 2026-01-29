namespace Loupedeck.LLHMPlugin.Services
{
    using System;
    using System.Linq;

    /// <summary>
    /// 하드웨어 자동 탐지 및 센서 경로 관리.
    /// 플러그인 시작 시 LHM에서 존재하는 CPU/GPU를 탐지하고 센서 경로를 캐싱합니다.
    /// </summary>
    public class HardwareRegistry
    {
        private static HardwareRegistry _instance;
        public static HardwareRegistry Instance => _instance;

        // 탐지 우선순위 (왼쪽이 높은 우선순위)
        private static readonly string[] CpuPrefixes = { "/amdcpu/", "/intelcpu/", "/genericcpu/" };
        private static readonly string[] GpuPrefixes = { "/gpu-nvidia/", "/gpu-amd/", "/gpu-intel/" };

        // 탐지된 하드웨어 경로 (예: "/amdcpu/0", "/gpu-nvidia/0")
        public string CpuPrefix { get; private set; }
        public string GpuPrefix { get; private set; }

        // 탐지 상태
        public bool IsCpuDetected => !string.IsNullOrEmpty(CpuPrefix);
        public bool IsGpuDetected => !string.IsNullOrEmpty(GpuPrefix);

        private HardwareRegistry() { }

        /// <summary>
        /// HardwareRegistry를 초기화하고 하드웨어를 탐지합니다.
        /// DataService가 데이터를 수신한 후 호출해야 합니다.
        /// </summary>
        public static void Initialize(LhmDataService dataService)
        {
            _instance = new HardwareRegistry();
            _instance.DetectHardware(dataService);
        }

        /// <summary>
        /// 하드웨어를 재탐지합니다. 센서가 변경되었을 때 호출할 수 있습니다.
        /// </summary>
        public void Refresh(LhmDataService dataService)
        {
            DetectHardware(dataService);
        }

        private void DetectHardware(LhmDataService dataService)
        {
            // CPU 탐지
            CpuPrefix = null;
            foreach (var prefix in CpuPrefixes)
            {
                var sensors = dataService.GetSensorsByPrefix(prefix);
                if (sensors.Count > 0)
                {
                    // 첫 번째 센서의 경로에서 디바이스 prefix 추출 (예: "/amdcpu/0")
                    CpuPrefix = ExtractDevicePrefix(sensors[0].SensorId);
                    break;
                }
            }

            // GPU 탐지
            GpuPrefix = null;
            foreach (var prefix in GpuPrefixes)
            {
                var sensors = dataService.GetSensorsByPrefix(prefix);
                if (sensors.Count > 0)
                {
                    GpuPrefix = ExtractDevicePrefix(sensors[0].SensorId);
                    break;
                }
            }
        }

        /// <summary>
        /// 센서 경로에서 디바이스 prefix를 추출합니다.
        /// 예: "/gpu-nvidia/0/temperature/0" → "/gpu-nvidia/0"
        /// </summary>
        private static string ExtractDevicePrefix(string sensorId)
        {
            if (string.IsNullOrEmpty(sensorId))
                return null;

            // 경로 분리: ["", "gpu-nvidia", "0", "temperature", "0"]
            var parts = sensorId.Split('/');
            if (parts.Length >= 3)
            {
                // "/{type}/{index}" 형태로 반환
                return $"/{parts[1]}/{parts[2]}";
            }
            return null;
        }

        // ============================================================
        // CPU 센서 경로 메서드
        // ============================================================

        /// <summary>CPU 온도 센서 경로. AMD: temperature/2 (Tctl/Tdie), Intel: temperature/0</summary>
        public string GetCpuTemperaturePath()
        {
            if (!IsCpuDetected) return null;

            // AMD CPU는 temperature/2가 Tctl/Tdie (실제 다이 온도)
            if (CpuPrefix.Contains("/amdcpu/"))
                return $"{CpuPrefix}/temperature/2";

            // Intel CPU는 temperature/0이 패키지 온도
            return $"{CpuPrefix}/temperature/0";
        }

        /// <summary>CPU 전체 사용률 센서 경로</summary>
        public string GetCpuLoadPath() => IsCpuDetected ? $"{CpuPrefix}/load/0" : null;

        /// <summary>CPU 전력 센서 경로</summary>
        public string GetCpuPowerPath() => IsCpuDetected ? $"{CpuPrefix}/power/0" : null;

        // ============================================================
        // GPU 센서 경로 메서드
        // ============================================================

        /// <summary>GPU 코어 온도 센서 경로</summary>
        public string GetGpuTemperaturePath() => IsGpuDetected ? $"{GpuPrefix}/temperature/0" : null;

        /// <summary>GPU 코어 사용률 센서 경로</summary>
        public string GetGpuLoadPath() => IsGpuDetected ? $"{GpuPrefix}/load/0" : null;

        /// <summary>GPU 전력 센서 경로</summary>
        public string GetGpuPowerPath() => IsGpuDetected ? $"{GpuPrefix}/power/0" : null;

        /// <summary>GPU VRAM 사용량 센서 경로 (MB)</summary>
        public string GetGpuVramUsedPath() => IsGpuDetected ? $"{GpuPrefix}/smalldata/1" : null;

        /// <summary>GPU VRAM 총량 센서 경로 (MB)</summary>
        public string GetGpuVramTotalPath() => IsGpuDetected ? $"{GpuPrefix}/smalldata/2" : null;
    }
}
