using UnityEngine;

namespace ProjectC.Runtime
{
    [CreateAssetMenu(fileName = "SO_MapSettings", menuName = "Scriptable Objects/Settings/Map")]
    public class MapSettingsData : ScriptableObject
    {
        [SerializeField, Min(1)] private int size = 30;

        [SerializeField] private float vertexSpacing = 1f;

        [SerializeField] private float randomHeight = 1.2f;

        [SerializeField, Min(0.001f)] private float noiseScale = 0.15f;

        [SerializeField, Min(1)] private int heightLevels = 8;

        [SerializeField] private Material planeMat = null;

        // MapController가 맵 생성에 필요한 값만 읽도록 설정 데이터를 캡슐화합니다.
        public int Size => size;

        public float VertexSpacing => vertexSpacing;

        public float RandomHeight => randomHeight;

        public float NoiseScale => noiseScale;

        public int HeightLevels => heightLevels;

        public Material PlaneMat => planeMat;

        public float ChunkSize => Size * VertexSpacing;
    }
}
