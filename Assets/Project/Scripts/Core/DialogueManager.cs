namespace ProjectC.Runtime
{
    /// <summary>
    /// 대화 시스템 구현을 연결할 자리만 제공하는 전역 관리자입니다.
    /// </summary>
    public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
    {
        /// <summary>
        /// DialogueManager가 전달할 대화 상태의 최소 껍데기입니다.
        /// </summary>
        public class Dialogue
        {
            public string speakerName;
            public string eventId;
            public string nextDialogueId;
            public int index;
        }

        /// <summary>
        /// 대화 매니저 인스턴스를 보장합니다.
        /// </summary>
        public static void Init()
        {
            GetInstance();
        }
    }
}
