using System.Collections.Generic;
using UnityEngine;

namespace ProjectC.Runtime
{
    public class RailSegment
    {
        public int ownerControllerInstanceId;
        public int instanceId;
        public GameObject railInst;
        public Vector2Int coord;

        public RailSegment(int ownerControllerInstanceId, GameObject rail, Vector2Int coord)
        {
            this.ownerControllerInstanceId = ownerControllerInstanceId;
            this.instanceId = rail.GetInstanceID();
            this.railInst = rail;
            this.coord = coord;
        }

        public int OwnerControllerInstanceId => ownerControllerInstanceId;

        public int InstanceId => instanceId;
    }

    public class RailController : MonoBehaviour
    {
        private const int RailChunkRadius = 1;

        private const int RailChunkLineCount = RailChunkRadius * 2 + 1;

        private const float RailUnitSpacing = 45f;

        [SerializeField] private MapSettingsData mapSettingsData = null;

        [SerializeField] private GameObject railPf = null;

        [SerializeField, Min(0f)] private float moveSpeed = 5f;

        private GameObject railRoot = null;

        private WorldManager worldManager = null;

        private readonly List<RailSegment> railSegments = new();

        private void OnEnable()
        {
            worldManager = WorldManager.GetInstance();
            worldManager.RailRegistered += HandleRailRegistered;
        }

        private void OnDisable()
        {
            if (worldManager == null) return;

            worldManager.RailRegistered -= HandleRailRegistered;
        }

        private void Start()
        {
            GenerateRails();
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (railSegments.Count == 0) return;

            float moveDistance = moveSpeed * Time.deltaTime;

            for (int i = 0; i < railSegments.Count; i++)
            {
                MoveRail(railSegments[i], moveDistance);
            }
        }

        public void GenerateRails()
        {
            worldManager ??= WorldManager.GetInstance();

            railRoot = new GameObject("RailRoot");
            railRoot.transform.SetParent(transform);

            int railSegmentCount = GetRailSegmentCount();
            int startCoordZ = -(railSegmentCount / 2);

            for (int i = railSegmentCount - 1; i >= 0; i--)
            {
                int coordZ = startCoordZ + i;
                Vector2Int coord = new Vector2Int(0, coordZ);

                // rail 인스턴싱
                GameObject railInst = Instantiate(railPf, railRoot.transform);
                railInst.name = $"Rail_({coord.x}, {coord.y})";
                railInst.transform.localPosition = new Vector3(0f, 0f, coordZ * RailUnitSpacing);

                // rail 소유권은 WorldManager에 등록하고, 컨트롤러는 콜백으로 목록만 동기화합니다.
                worldManager.RegisterRail(GetInstanceID(), railInst, coord);
            }
        }

        public bool TryGetRail(Vector2Int coord, out RailSegment railSegment)
        {
            railSegment = null;

            for (int i = 0; i < railSegments.Count; i++)
            {
                if (railSegments[i].coord != coord) continue;

                railSegment = railSegments[i];
                return true;
            }

            return false;
        }

        private void MoveRail(RailSegment railSegment, float moveDistance)
        {
            Transform railTransform = railSegment.railInst.transform;
            railTransform.localPosition += Vector3.back * moveDistance;

            int railSegmentCount = GetRailSegmentCount();
            float backLimitZ = railSegmentCount * RailUnitSpacing * -0.5f;

            // 뒤쪽 경계를 넘은 해당 레일만 앞쪽으로 순환시킵니다.
            while (railTransform.localPosition.z <= backLimitZ)
            {
                Vector3 localPosition = railTransform.localPosition;
                localPosition.z += railSegmentCount * RailUnitSpacing;
                railTransform.localPosition = localPosition;

                Vector2Int newCoord = new Vector2Int(railSegment.coord.x, railSegment.coord.y + railSegmentCount);
                railSegment.railInst.name = $"Rail_({newCoord.x}, {newCoord.y})";
                worldManager.UpdateRailCoord(railSegment, newCoord);
            }
        }

        private int GetRailSegmentCount()
        {
            // Rail 개수는 Map ChunkSize를 45 단위로 나눈 뒤 3개 청크 구간에 맞춥니다.
            return Mathf.RoundToInt(mapSettingsData.ChunkSize / RailUnitSpacing) * RailChunkLineCount;
        }

        private void HandleRailRegistered(RailSegment railSegment)
        {
            if (railSegment.OwnerControllerInstanceId != GetInstanceID()) return;

            railSegments.Add(railSegment);
        }
    }

}
