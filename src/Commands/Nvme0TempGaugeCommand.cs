namespace Loupedeck.LLHMPlugin.Commands
{
    using System;
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Helpers;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// NVMe #0 온도를 아치형 게이지로 표시합니다.
    /// 게이지 범위 20~100°C. 60°C까지 초록, 76°C까지 노란색, 이상 빨간색.
    /// </summary>
    public class Nvme0TempGaugeCommand : BaseSensorCommand
    {
        private const string SensorId = "/nvme/0/temperature/0";
        private const double MinTemp = 20;
        private const double MaxTemp = 100;

        private static readonly BitmapColor GrayColor = new BitmapColor(80, 80, 80);
        private static readonly BitmapColor GreenColor = new BitmapColor(0, 200, 80);
        private static readonly BitmapColor YellowColor = new BitmapColor(255, 220, 0);
        private static readonly BitmapColor RedColor = new BitmapColor(255, 50, 50);

        public Nvme0TempGaugeCommand()
            : base("NVMe #0 (Gauge)", "NVMe #0 Temp Gauge View", "HW Monitor - NVMe")
        {
        }

        protected override string GetLabel() => "NVMe#0";

        protected override void DrawSensorData(BitmapBuilder builder, LhmDataService service)
        {
            var sensor = service.GetSensor(SensorId);
            if (sensor == null)
            {
                DrawValue(builder, "N/A", DisplayHelper.OfflineColor);
                return;
            }

            var temp = LhmDataService.ParseValue(sensor.Value);
            DrawGauge(builder, temp);
        }

        private void DrawGauge(BitmapBuilder builder, double temp)
        {
            var w = builder.Width;
            var h = builder.Height;

            var cx = w / 2.0;
            var cy = h / 2.0 + 8;
            var outerR = (w / 2.0) - 8;
            var innerR = outerR - 10;

            var clamped = Math.Max(MinTemp, Math.Min(MaxTemp, temp));
            var ratio = (clamped - MinTemp) / (MaxTemp - MinTemp);

            var bg = DisplayHelper.BackgroundColor;
            const int samples = 4;
            var step = 1.0 / samples;

            var scanTop = (int)(cy - outerR) - 1;
            var scanLeft = (int)(cx - outerR) - 1;
            var scanRight = (int)(cx + outerR) + 1;
            var scanBottom = (int)cy + 1;

            for (var py = scanTop; py <= scanBottom; py++)
            {
                for (var px = scanLeft; px <= scanRight; px++)
                {
                    double rSum = 0, gSum = 0, bSum = 0;
                    var arcHits = 0;

                    for (var sy = 0; sy < samples; sy++)
                    {
                        for (var sx = 0; sx < samples; sx++)
                        {
                            var spx = px + (sx + 0.5) * step;
                            var spy = py + (sy + 0.5) * step;

                            var dx = spx - cx;
                            var dy = spy - cy;
                            var dist = Math.Sqrt(dx * dx + dy * dy);

                            if (dist < innerR || dist > outerR || dy > 0)
                            {
                                rSum += bg.R;
                                gSum += bg.G;
                                bSum += bg.B;
                                continue;
                            }

                            arcHits++;
                            var angle = Math.Atan2(-dy, dx);
                            var progress = 1.0 - (angle / Math.PI);

                            BitmapColor c;
                            if (progress <= ratio)
                            {
                                var t = MinTemp + progress * (MaxTemp - MinTemp);
                                c = t >= 75 ? RedColor
                                  : t >= 60 ? YellowColor
                                  : GreenColor;
                            }
                            else
                            {
                                // 미달 영역도 온도 구간별 색상을 희미하게 표시
                                var t = MinTemp + progress * (MaxTemp - MinTemp);
                                var baseColor = t >= 75 ? RedColor
                                              : t >= 60 ? YellowColor
                                              : GreenColor;

                                // 희미한 색상: 원본 색상 30% + 배경색 70% 블렌딩
                                const double opacity = 0.3;
                                c = new BitmapColor(
                                    (byte)(baseColor.R * opacity + bg.R * (1 - opacity)),
                                    (byte)(baseColor.G * opacity + bg.G * (1 - opacity)),
                                    (byte)(baseColor.B * opacity + bg.B * (1 - opacity))
                                );
                            }

                            rSum += c.R;
                            gSum += c.G;
                            bSum += c.B;
                        }
                    }

                    if (arcHits == 0)
                        continue;

                    var total = (double)(samples * samples);
                    builder.FillRectangle(px, py, 1, 1,
                        new BitmapColor((byte)(rSum / total), (byte)(gSum / total), (byte)(bSum / total)));
                }
            }

            // 온도 구간 구분 호 그리기
            DrawThresholdArcs(builder, cx, cy, outerR);

            var valueColor = temp >= 75 ? RedColor
                           : temp >= 60 ? YellowColor
                           : GreenColor;
            builder.DrawText($"{temp:F0}\u00b0C", 0, (int)(cy - 16), w, 18,
                valueColor, 14);

            builder.DrawText(GetLabel(), 0, (int)(cy + 2), w, 14,
                DisplayHelper.LabelColor, 11);
        }

        /// <summary>
        /// 온도 구간별로 색상이 구분된 얇은 호를 그립니다.
        /// </summary>
        private void DrawThresholdArcs(BitmapBuilder builder, double cx, double cy, double outerR)
        {
            var gap = 2;
            var thickness = 2;
            var arcRadius = outerR + gap + thickness / 2.0;

            // 구간별로 호 그리기 (NVMe: 20~59, 61~74, 76~100)
            DrawArcSegment(builder, cx, cy, arcRadius, thickness, 20, 59, GreenColor);
            DrawArcSegment(builder, cx, cy, arcRadius, thickness, 61, 74, YellowColor);
            DrawArcSegment(builder, cx, cy, arcRadius, thickness, 76, 100, RedColor);
        }

        /// <summary>
        /// 특정 온도 범위에 해당하는 호 세그먼트를 그립니다 (안티앨리어싱 적용).
        /// </summary>
        private void DrawArcSegment(BitmapBuilder builder, double cx, double cy, double radius,
                                   double thickness, double startTemp, double endTemp, BitmapColor color)
        {
            var startProgress = (startTemp - MinTemp) / (MaxTemp - MinTemp);
            var endProgress = (endTemp - MinTemp) / (MaxTemp - MinTemp);

            var startAngle = (1.0 - endProgress) * Math.PI;
            var endAngle = (1.0 - startProgress) * Math.PI;

            var halfThickness = thickness / 2.0;
            var innerR = radius - halfThickness;
            var outerR = radius + halfThickness;

            var bg = DisplayHelper.BackgroundColor;
            const int samples = 4;
            var step = 1.0 / samples;

            var scanLeft = (int)(cx - outerR - 2);
            var scanRight = (int)(cx + outerR + 2);
            var scanTop = (int)(cy - outerR - 2);
            var scanBottom = (int)cy + 1;

            for (var py = scanTop; py <= scanBottom; py++)
            {
                for (var px = scanLeft; px <= scanRight; px++)
                {
                    if (px < 0 || px >= builder.Width || py < 0 || py >= builder.Height)
                        continue;

                    double rSum = 0, gSum = 0, bSum = 0;
                    var arcHits = 0;

                    for (var sy = 0; sy < samples; sy++)
                    {
                        for (var sx = 0; sx < samples; sx++)
                        {
                            var spx = px + (sx + 0.5) * step;
                            var spy = py + (sy + 0.5) * step;

                            var dx = spx - cx;
                            var dy = spy - cy;
                            var dist = Math.Sqrt(dx * dx + dy * dy);
                            var sampleAngle = Math.Atan2(-dy, dx);

                            if (sampleAngle >= startAngle && sampleAngle <= endAngle &&
                                dist >= innerR && dist <= outerR && dy < 0)
                            {
                                arcHits++;
                                rSum += color.R;
                                gSum += color.G;
                                bSum += color.B;
                            }
                            else
                            {
                                rSum += bg.R;
                                gSum += bg.G;
                                bSum += bg.B;
                            }
                        }
                    }

                    if (arcHits == 0)
                        continue;

                    var total = (double)(samples * samples);
                    builder.FillRectangle(px, py, 1, 1,
                        new BitmapColor((byte)(rSum / total), (byte)(gSum / total), (byte)(bSum / total)));
                }
            }
        }
    }
}
