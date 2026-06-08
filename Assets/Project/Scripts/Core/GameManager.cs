using DG.Tweening;
using UnityEngine;

namespace ProjectC.Runtime
{
    /// <summary>
    /// 클라이언트 전역 매니저들의 초기화 순서를 연결하는 진입점입니다.
    /// </summary>
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void OnBeforeSplashSceneLoaded()
        {
            GetInstance();
            InputManager.Init();
            DOTween.Init();
            ResourceManager.Init();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoaded()
        {
            UIManager.Init();
            WorldManager.Init();
            DialogueManager.Init();
            EventManager.Init();
        }
    }
}
