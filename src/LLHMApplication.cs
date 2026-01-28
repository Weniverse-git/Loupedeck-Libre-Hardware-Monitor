namespace Loupedeck.LLHMPlugin
{
    using Loupedeck;

    /// <summary>
    /// LLHM 플러그인의 ClientApplication 구현.
    /// Loupedeck SDK는 모든 플러그인에 Plugin과 ClientApplication 두 클래스를 요구합니다.
    /// 이 플러그인은 특정 애플리케이션에 연결되지 않는 유니버설 플러그인이므로
    /// 최소한의 구현만 제공합니다.
    /// </summary>
    public class LLHMApplication : ClientApplication
    {
        public LLHMApplication()
        {
        }
    }
}
