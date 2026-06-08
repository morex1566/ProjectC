using UnityEngine;

namespace ProjectC.Runtime
{
    /// <summary>
    /// 한 프레임 동안 수집된 플레이어 입력 값을 전달하는 스냅샷입니다.
    /// </summary>
    public struct InputSnapshot
    {
        public Vector2 move;
        public Vector2 look;
        public bool attackPressed;
        public bool rollPressed;
        public bool reloadPressed;

        public bool IsEmpty => Equals(default(InputSnapshot));

        /// <summary>
        /// 이번 프레임 입력 스냅샷을 반환하고 트리거 입력을 소비 상태로 초기화합니다.
        /// </summary>
        public InputSnapshot Consume()
        {
            InputSnapshot value = this;
            this = default;
            move = value.move;
            look = value.look;

            return value;
        }
    }

    /// <summary>
    /// Input System 액션 맵의 생성과 수명만 관리하는 전역 입력 관리자입니다.
    /// </summary>
    public class InputManager : MonoBehaviourSingleton<InputManager>
    {
        public static InputMappingContext InputMappingContext { get; private set; }

        /// <summary>
        /// 입력 매핑 컨텍스트를 생성하고 활성화합니다.
        /// </summary>
        public static void Init()
        {
            GetInstance();
            if (InputMappingContext != null) return;

            InputMappingContext = new InputMappingContext();
            InputMappingContext.Enable();
        }

        /// <summary>
        /// 매니저 제거 시 InputActionAsset 참조를 정리합니다.
        /// </summary>
        protected override void OnDestroy()
        {
            InputMappingContext?.Disable();
            InputMappingContext?.Dispose();
            InputMappingContext = null;

            base.OnDestroy();
        }
    }
}
