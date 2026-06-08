namespace ProjectC.Runtime
{
    /// <summary>
    /// 런타임 리소스 로딩 구현을 연결할 자리만 제공하는 전역 관리자입니다.
    /// </summary>
    public class ResourceManager : MonoBehaviourSingleton<ResourceManager>
    {
        /// <summary>
        /// 리소스 매니저 인스턴스를 보장합니다.
        /// </summary>
        public static void Init()
        {
            GetInstance();
        }
    }
}
