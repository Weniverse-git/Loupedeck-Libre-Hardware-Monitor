namespace Loupedeck.LLHMPlugin.Commands
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// 네트워크 Upload/Download 속도를 하나의 터치 버튼에 통합 표시합니다.
    /// 동적 탐지로 NIC를 자동 감지합니다.
    /// </summary>
    public class NetworkSpeedCommand : BaseSensorCommand
    {
        private static readonly BitmapColor UploadColor = new BitmapColor(0, 200, 180);
        private static readonly BitmapColor DownloadColor = new BitmapColor(0, 180, 220);
        private static readonly BitmapColor InactiveColor = new BitmapColor(0, 120, 120);

        public NetworkSpeedCommand()
            : base("Network Speed", "Network Upload/Download Speed", "HW Monitor - ETC")
        {
        }

        protected override string GetLabel() => "NET";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var registry = HardwareRegistry.Instance;

            // 모든 NIC 중 트래픽이 가장 높은 NIC를 동적 선택
            var bestNic = registry?.GetBestNicSpeeds(service);

            if (bestNic == null)
            {
                builder.DrawText("NET", 0, 4, builder.Width, 18,
                    DisplayHelper.LabelColor, 13);
                builder.DrawText("N/A", 0, 28, builder.Width, 30,
                    DisplayHelper.OfflineColor, 16);
                return;
            }

            var uploadText = DisplayHelper.FormatNetworkSpeed(bestNic.UploadSpeed);
            var downloadText = DisplayHelper.FormatNetworkSpeed(bestNic.DownloadSpeed);

            var uploadDisplayColor = bestNic.UploadSpeed > 0 ? UploadColor : InactiveColor;
            var downloadDisplayColor = bestNic.DownloadSpeed > 0 ? DownloadColor : InactiveColor;

            // 레이아웃: NET 라벨 + Upload + Download
            builder.DrawText("NET", 0, 4, builder.Width, 18,
                DisplayHelper.LabelColor, 13);
            builder.DrawText($"\u2191 {uploadText}", 0, 24, builder.Width, 20,
                uploadDisplayColor, 16);
            builder.DrawText($"\u2193 {downloadText}", 0, 46, builder.Width, 20,
                downloadDisplayColor, 16);
        }

        protected override void DrawOfflineState(BitmapBuilder builder)
        {
            builder.DrawText("NET", 0, 4, builder.Width, 18,
                DisplayHelper.LabelColor, 13);
            builder.DrawText("\u2191 ---", 0, 24, builder.Width, 20,
                DisplayHelper.OfflineColor, 16);
            builder.DrawText("\u2193 ---", 0, 46, builder.Width, 20,
                DisplayHelper.OfflineColor, 16);
        }
    }
}
