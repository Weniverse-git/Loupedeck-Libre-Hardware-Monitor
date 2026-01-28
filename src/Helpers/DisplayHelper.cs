namespace Loupedeck.LLHMPlugin.Helpers
{
    using System;
    using Loupedeck;

    /// <summary>
    /// 센서 값의 색상 결정, 포맷팅 등 디스플레이 관련 유틸리티.
    /// </summary>
    public static class DisplayHelper
    {
        /// <summary>
        /// 온도에 따라 색상을 반환합니다 (컴포넌트별 기준값 지정).
        /// 녹색 → 노랑(yellowThreshold 이상) → 빨강(redThreshold 이상)
        /// </summary>
        public static BitmapColor GetTemperatureColor(double temperature, double yellowThreshold, double redThreshold)
        {
            if (temperature >= redThreshold)
                return new BitmapColor(255, 60, 60);    // 빨강
            if (temperature >= yellowThreshold)
                return new BitmapColor(255, 180, 0);    // 주황/노랑
            return new BitmapColor(0, 200, 80);         // 녹색
        }

        /// <summary>
        /// 사용률(%)에 따라 색상을 반환합니다.
        /// 녹색(~60%) → 노랑(60~85%) → 빨강(85%~)
        /// </summary>
        public static BitmapColor GetLoadColor(double loadPercent)
        {
            if (loadPercent >= 85)
                return new BitmapColor(255, 60, 60);
            if (loadPercent >= 60)
                return new BitmapColor(255, 180, 0);
            return new BitmapColor(0, 200, 80);
        }

        /// <summary>
        /// 오프라인 상태 표시 색상.
        /// </summary>
        public static BitmapColor OfflineColor => new BitmapColor(120, 120, 120);

        /// <summary>
        /// 배경 색상.
        /// </summary>
        public static BitmapColor BackgroundColor => BitmapColor.Black;

        /// <summary>
        /// 라벨 텍스트 색상.
        /// </summary>
        public static BitmapColor LabelColor => new BitmapColor(180, 180, 180);

        /// <summary>
        /// 값 문자열에서 숫자를 추출하고 단위를 붙여 간결하게 표시합니다.
        /// 예: "62.9 °C" → "62.9°C", "4.2 %" → "4.2%"
        /// </summary>
        public static string FormatSensorValue(string rawValue, string fallback = "N/A")
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return fallback;

            // 공백 제거하여 간결하게
            return rawValue.Trim().Replace(" ", "");
        }

        /// <summary>
        /// 메모리/데이터 크기를 간결하게 표시합니다.
        /// 예: "23218.0 MB" → "22.7 GB"
        /// </summary>
        public static string FormatMemoryValue(double valueMB)
        {
            if (valueMB >= 1024)
                return $"{valueMB / 1024:F1}GB";
            return $"{valueMB:F0}MB";
        }
    }
}
