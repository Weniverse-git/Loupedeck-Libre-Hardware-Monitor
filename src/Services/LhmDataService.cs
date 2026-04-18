namespace Loupedeck.LLHMPlugin.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;

    /// <summary>
    /// Libre Hardware Monitor의 Remote Web Server에서 센서 데이터를 주기적으로 가져오는 서비스.
    /// JSON 트리를 재귀적으로 탐색하여 SensorId 기반의 플랫 딕셔너리로 변환합니다.
    /// </summary>
    public class LhmDataService : IDisposable
    {
        private readonly string _url;
        private readonly HttpClient _httpClient;
        private Timer _pollTimer;
        private bool _disposed;

        // SensorId → SensorNode 매핑 (스레드 안전하게 교체)
        private volatile Dictionary<string, SensorNode> _sensors = new Dictionary<string, SensorNode>();

        private static LhmDataService _instance;

        public static LhmDataService Instance => _instance;
        /// <summary>연결 상태. true면 데이터 수신 중.</summary>
        public bool IsConnected { get; private set; }

        /// <summary>마지막 에러 메시지.</summary>
        public string LastError { get; private set; } = "";

        /// <summary>데이터가 갱신되었을 때 발생하는 이벤트.</summary>
        public event Action DataUpdated;

        public LhmDataService(string url = "http://localhost:8085/data.json")
        {
            _url = url;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        public void Start()
        {
            // 1초마다 폴링 (초기 500ms 후 시작)
            _pollTimer = new Timer(PollData, null, 500, 1000);
        }

        public void Stop()
        {
            _pollTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// SensorId로 센서 데이터를 조회합니다.
        /// </summary>
        /// <param name="sensorId">예: "/amdcpu/0/temperature/2"</param>
        /// <returns>센서 노드 또는 null</returns>
        public SensorNode GetSensor(string sensorId)
        {
            var sensors = _sensors;
            if (sensors != null && sensors.TryGetValue(sensorId, out var node))
            {
                return node;
            }
            return null;
        }

        /// <summary>
        /// SensorId 접두사로 시작하는 모든 센서를 조회합니다.
        /// 예: "/gpu-nvidia/0/temperature" → 해당 경로 아래 모든 온도 센서
        /// </summary>
        public List<SensorNode> GetSensorsByPrefix(string prefix)
        {
            var result = new List<SensorNode>();
            var sensors = _sensors;
            if (sensors != null)
            {
                foreach (var kvp in sensors)
                {
                    if (kvp.Key != null && kvp.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(kvp.Value);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 특정 값 문자열에서 숫자만 추출합니다.
        /// 예: "62.9 °C" → 62.9, "4.2 %" → 4.2
        /// </summary>
        public static double ParseValue(string valueStr)
        {
            if (string.IsNullOrWhiteSpace(valueStr))
                return 0;

            // 숫자와 소수점, 음수 부호만 추출
            var numStr = "";
            foreach (var ch in valueStr)
            {
                if (char.IsDigit(ch) || ch == '.' || ch == '-')
                    numStr += ch;
                else if (numStr.Length > 0)
                    break; // 숫자 끝나면 중단
            }

            if (double.TryParse(numStr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var val))
            {
                return val;
            }
            return 0;
        }

        private async void PollData(object state)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(_url);
                var root = JsonSerializer.Deserialize<SensorNode>(response);

                if (root != null)
                {
                    var newSensors = new Dictionary<string, SensorNode>();
                    TraverseTree(root, newSensors);
                    _sensors = newSensors;
                    IsConnected = true;
                    LastError = "";
                    DataUpdated?.Invoke();
                }
            }
            catch (HttpRequestException ex)
            {
                IsConnected = false;
                LastError = $"Connection failed: {ex.Message}";
            }
            catch (TaskCanceledException)
            {
                IsConnected = false;
                LastError = "Request timeout";
            }
            catch (JsonException ex)
            {
                IsConnected = false;
                LastError = $"JSON parse error: {ex.Message}";
            }
            catch (Exception ex)
            {
                IsConnected = false;
                LastError = $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// JSON 트리를 재귀적으로 탐색하여 SensorId가 있는 노드만 딕셔너리에 추가합니다.
        /// </summary>
        private void TraverseTree(SensorNode node, Dictionary<string, SensorNode> sensors)
        {
            if (!string.IsNullOrEmpty(node.SensorId))
            {
                sensors[node.SensorId] = node;
            }

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    TraverseTree(child, sensors);
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _pollTimer?.Dispose();
                _httpClient?.Dispose();
            }
        }
    }
}
