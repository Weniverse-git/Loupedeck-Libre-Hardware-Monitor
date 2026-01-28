namespace Loupedeck.LLHMPlugin
{
    using Loupedeck;
    using Loupedeck.LLHMPlugin.Services;

    /// <summary>
    /// Libre Hardware Monitor 데이터를 Loupedeck/Razer Stream Controller에 표시하는 플러그인.
    /// LHM의 Remote Web Server (http://localhost:8085/data.json)에서 센서 데이터를 가져옵니다.
    /// </summary>
    public class LLHMPlugin : Plugin
    {
        /// <summary>LHM 데이터 서비스 (싱글톤). 모든 Command에서 공유.</summary>
        internal static LhmDataService DataService { get; private set; }

        public override bool HasNoApplication => true;
        public override bool UsesApplicationApiOnly => true;

        public override void Load()
        {
            DataService = new LhmDataService("http://localhost:8085/data.json");
            DataService.Start();
        }

        public override void Unload()
        {
            DataService?.Stop();
            DataService?.Dispose();
            DataService = null;
        }
    }
}
