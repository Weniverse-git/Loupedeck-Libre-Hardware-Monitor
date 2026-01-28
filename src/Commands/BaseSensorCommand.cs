namespace Loupedeck.LLHMPlugin.Commands
{
    using System;
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;
    using MonitorPlugin = Loupedeck.LLHMPlugin.LLHMPlugin;

    /// <summary>
    /// 모든 센서 표시 Command의 공통 베이스 클래스.
    /// 데이터 갱신 시 자동으로 버튼 이미지를 업데이트합니다.
    /// </summary>
    public abstract class BaseSensorCommand : PluginDynamicCommand
    {
        private bool _subscribed;

        protected BaseSensorCommand(string displayName, string description, string groupName)
            : base("", description, groupName)
        {
            this.DisplayName = displayName;
            this.SetWidget(true);
        }

        private void EnsureSubscribed()
        {
            if (_subscribed)
                return;

            var service = MonitorPlugin.DataService;
            if (service != null)
            {
                service.DataUpdated += OnDataUpdated;
                _subscribed = true;
            }
        }

        private void OnDataUpdated()
        {
            this.ActionImageChanged();
        }

        protected override void RunCommand(string actionParameter)
        {
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            EnsureSubscribed();

            using (var builder = new BitmapBuilder(imageSize))
            {
                builder.Clear(DisplayHelper.BackgroundColor);

                var service = MonitorPlugin.DataService;
                if (service == null || !service.IsConnected)
                {
                    DrawOfflineState(builder);
                }
                else
                {
                    DrawSensorData(builder, service);
                }

                return builder.ToImage();
            }
        }

        protected abstract void DrawSensorData(BitmapBuilder builder, LhmDataService service);

        protected virtual void DrawOfflineState(BitmapBuilder builder)
        {
            builder.DrawText("Offline", 0, 15, builder.Width, builder.Height - 38,
                DisplayHelper.OfflineColor, 13);
            builder.DrawText(GetLabel(), 0, builder.Height - 40, builder.Width, 20,
                DisplayHelper.LabelColor, 11);
        }

        protected abstract string GetLabel();

        protected void DrawValue(BitmapBuilder builder, string valueText, BitmapColor valueColor)
        {
            builder.DrawText(valueText, 0, 15, builder.Width, builder.Height - 38,
                valueColor, 19);
            builder.DrawText(GetLabel(), 0, builder.Height - 40, builder.Width, 20,
                DisplayHelper.LabelColor, 11);
        }
    }
}
