using UnityEngine;

namespace ProjectC.Runtime
{
    /// <summary>
    /// UI 생성/해제 구현을 연결할 자리만 제공하는 전역 관리자입니다.
    /// </summary>
    public class UIManager : MonoBehaviourSingleton<UIManager>
    {
        public enum RenderSpace
        {
            Overlay,
            World,
            Camera
        }

        /// <summary>
        /// UI 매니저 인스턴스를 보장합니다.
        /// </summary>
        public static void Init()
        {
            GetInstance();
        }

        /// <summary>
        /// UI 좌표 변환 구현이 추가될 연결점입니다.
        /// </summary>
        public static Vector3 WorldPosToUIPos(Vector3 worldPosition, RectTransform targetRect = null, Camera worldCamera = null)
        {
            return worldPosition;
        }

        /// <summary>
        /// UI 닫기 구현이 추가될 연결점입니다.
        /// </summary>
        public static void Close(int instanceId)
        {
        }
    }
}
