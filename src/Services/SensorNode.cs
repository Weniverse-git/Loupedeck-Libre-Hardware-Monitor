namespace Loupedeck.LHMMonitorPlugin.Services
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Libre Hardware Monitor JSON 트리의 각 노드를 나타내는 데이터 모델.
    /// /data.json 응답의 재귀적 Children 구조를 그대로 역직렬화합니다.
    /// </summary>
    public class SensorNode
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("Text")]
        public string Text { get; set; } = "";

        [JsonPropertyName("Min")]
        public string Min { get; set; } = "";

        [JsonPropertyName("Value")]
        public string Value { get; set; } = "";

        [JsonPropertyName("Max")]
        public string Max { get; set; } = "";

        [JsonPropertyName("SensorId")]
        public string SensorId { get; set; }

        [JsonPropertyName("Type")]
        public string Type { get; set; }

        [JsonPropertyName("ImageURL")]
        public string ImageURL { get; set; }

        [JsonPropertyName("Children")]
        public List<SensorNode> Children { get; set; } = new List<SensorNode>();
    }
}
