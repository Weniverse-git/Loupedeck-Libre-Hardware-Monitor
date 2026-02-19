namespace Loupedeck.LLHMPlugin.Services
{
    using System;
    using System.Collections.Generic;
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
        private const string NicSensorPrefix = "/nic/";

        // 탐지된 하드웨어 경로 (예: "/amdcpu/0", "/gpu-nvidia/0")
        public string CpuPrefix { get; private set; }
        public string GpuPrefix { get; private set; }
        public string NicPrefix { get; private set; }

        // NIC 센서 경로 캐싱 (단일 NIC 폴백용)
        private string _uploadSpeedSensorId;
        private string _downloadSpeedSensorId;

        // 모든 NIC의 Upload/Download 센서 경로 (동적 선택용)
        private List<NicSpeedPair> _allNicSpeedPairs = new List<NicSpeedPair>();

        /// <summary>NIC별 Upload/Download 센서 경로 쌍</summary>
        public class NicSpeedPair
        {
            public string UploadSensorId { get; set; }
            public string DownloadSensorId { get; set; }
        }

        // 탐지 상태
        public bool IsCpuDetected => !string.IsNullOrEmpty(CpuPrefix);
        public bool IsGpuDetected => !string.IsNullOrEmpty(GpuPrefix);
        public bool IsNicDetected => !string.IsNullOrEmpty(NicPrefix);

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

            // GPU 탐지 - Fan 센서가 있는 GPU 우선 선택 (dGPU vs APU 구분)
            GpuPrefix = null;
            var bestGpuWithFan = (string)null;
            var maxFanGpuSensors = 0;
            var bestGpuFallback = (string)null;
            var maxFallbackSensors = 0;

            foreach (var prefix in GpuPrefixes)
            {
                var sensors = dataService.GetSensorsByPrefix(prefix);
                if (sensors.Count == 0)
                    continue;

                // 각 GPU 디바이스별로 그룹핑
                var deviceGroups = sensors
                    .Select(s => new { Sensor = s, DevicePrefix = ExtractDevicePrefix(s.SensorId) })
                    .Where(x => x.DevicePrefix != null)
                    .GroupBy(x => x.DevicePrefix);

                foreach (var group in deviceGroups)
                {
                    var devicePrefix = group.Key;
                    var sensorCount = group.Count();
                    var hasFan = group.Any(x => x.Sensor.SensorId.Contains("/fan/"));

                    if (hasFan)
                    {
                        // Fan이 있는 GPU(dGPU) 중 센서가 가장 많은 것 선택
                        if (sensorCount > maxFanGpuSensors)
                        {
                            maxFanGpuSensors = sensorCount;
                            bestGpuWithFan = devicePrefix;
                        }
                    }
                    else
                    {
                        // Fan이 없는 GPU(APU/iGPU) - 폴백용
                        if (sensorCount > maxFallbackSensors)
                        {
                            maxFallbackSensors = sensorCount;
                            bestGpuFallback = devicePrefix;
                        }
                    }
                }
            }

            // Fan이 있는 dGPU 우선, 없으면 센서 수 기반 폴백
            GpuPrefix = bestGpuWithFan ?? bestGpuFallback;

            // NIC 탐지 - 모든 NIC의 throughput 센서를 수집
            NicPrefix = null;
            _uploadSpeedSensorId = null;
            _downloadSpeedSensorId = null;
            _allNicSpeedPairs = new List<NicSpeedPair>();

            var nicSensors = dataService.GetSensorsByPrefix(NicSensorPrefix);
            if (nicSensors.Count > 0)
            {
                // 모든 NIC 디바이스 prefix 추출
                var nicDevicePrefixes = nicSensors
                    .Select(s => ExtractDevicePrefix(s.SensorId))
                    .Where(p => p != null)
                    .Distinct()
                    .ToList();

                // 각 NIC에서 Upload/Download throughput 센서 찾기
                foreach (var nicDevicePrefix in nicDevicePrefixes)
                {
                    var throughputPrefix = $"{nicDevicePrefix}/throughput/";
                    var throughputSensors = dataService.GetSensorsByPrefix(throughputPrefix);

                    string uploadId = null;
                    string downloadId = null;

                    foreach (var sensor in throughputSensors)
                    {
                        if (sensor.Text != null)
                        {
                            var text = sensor.Text.ToLowerInvariant();
                            if (text.Contains("upload") && uploadId == null)
                                uploadId = sensor.SensorId;
                            else if (text.Contains("download") && downloadId == null)
                                downloadId = sensor.SensorId;
                        }
                    }

                    if (uploadId != null || downloadId != null)
                    {
                        _allNicSpeedPairs.Add(new NicSpeedPair
                        {
                            UploadSensorId = uploadId,
                            DownloadSensorId = downloadId
                        });
                    }
                }

                // 폴백: 첫 번째 NIC를 기본으로 설정
                if (_allNicSpeedPairs.Count > 0)
                {
                    NicPrefix = nicDevicePrefixes[0];
                    _uploadSpeedSensorId = _allNicSpeedPairs[0].UploadSensorId;
                    _downloadSpeedSensorId = _allNicSpeedPairs[0].DownloadSensorId;
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

        // ============================================================
        // NIC 센서 경로 메서드
        // ============================================================

        /// <summary>NIC Upload Speed 센서 경로 (폴백: 첫 번째 NIC)</summary>
        public string GetNicUploadSpeedPath() => _uploadSpeedSensorId;

        /// <summary>NIC Download Speed 센서 경로 (폴백: 첫 번째 NIC)</summary>
        public string GetNicDownloadSpeedPath() => _downloadSpeedSensorId;

        /// <summary>
        /// 현재 트래픽이 가장 높은 NIC의 Upload/Download 속도를 반환합니다.
        /// 모든 NIC의 throughput을 비교하여 (Upload+Download) 합계가 가장 큰 NIC를 선택합니다.
        /// 비용: 딕셔너리 조회 몇 회 (NIC 수 x 2) - 추가 HTTP 요청 없음.
        /// </summary>
        public NicSpeedResult GetBestNicSpeeds(LhmDataService service)
        {
            if (_allNicSpeedPairs.Count == 0)
                return null;

            // NIC가 1개면 바로 반환
            if (_allNicSpeedPairs.Count == 1)
            {
                var pair = _allNicSpeedPairs[0];
                return new NicSpeedResult
                {
                    UploadSpeed = GetSensorValue(service, pair.UploadSensorId),
                    DownloadSpeed = GetSensorValue(service, pair.DownloadSensorId)
                };
            }

            // 모든 NIC 비교 → 가장 트래픽 높은 NIC 선택
            double bestTotal = -1;
            NicSpeedResult bestResult = null;

            foreach (var pair in _allNicSpeedPairs)
            {
                var upload = GetSensorValue(service, pair.UploadSensorId);
                var download = GetSensorValue(service, pair.DownloadSensorId);
                var total = upload + download;

                if (total > bestTotal)
                {
                    bestTotal = total;
                    bestResult = new NicSpeedResult
                    {
                        UploadSpeed = upload,
                        DownloadSpeed = download
                    };
                }
            }

            return bestResult;
        }

        /// <summary>동적 NIC 선택 결과</summary>
        public class NicSpeedResult
        {
            public double UploadSpeed { get; set; }
            public double DownloadSpeed { get; set; }
        }

        private static double GetSensorValue(LhmDataService service, string sensorId)
        {
            if (sensorId == null) return 0;
            var sensor = service.GetSensor(sensorId);
            return sensor != null ? LhmDataService.ParseValue(sensor.Value) : 0;
        }
    }
}
