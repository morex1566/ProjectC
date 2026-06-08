using UnityEngine;

namespace ProjectC.Runtime
{
    /// <summary>
    /// UIManager 설정 에셋의 연결 지점입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "SO_UIManagerSettings", menuName = "Scriptable Objects/Settings/UIManager")]
    public class UIManagerSettingsData : ScriptableObject
    {
        [SerializeField] private Sprite cursorShape;

        public Sprite CursorShape => cursorShape;
    }
}
