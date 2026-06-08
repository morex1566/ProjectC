using Cysharp.Threading.Tasks;

namespace ProjectC.Runtime
{
    /// <summary>
    /// 게임 전역 이벤트 실행 구현을 연결할 자리만 제공하는 전역 관리자입니다.
    /// </summary>
    public class EventManager : MonoBehaviourSingleton<EventManager>
    {
        /// <summary>
        /// 이벤트 매니저 인스턴스를 보장합니다.
        /// </summary>
        public static void Init()
        {
            GetInstance();
        }

        /// <summary>
        /// 이벤트 실행 구현이 추가될 연결점입니다.
        /// </summary>
        public static UniTask TriggerAsync(string eventKey)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 이벤트 닫기 구현이 추가될 연결점입니다.
        /// </summary>
        public static void Close(int instanceId)
        {
        }
    }
}
