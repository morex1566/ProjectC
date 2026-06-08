using System.Collections.Generic;
using UnityEngine;

namespace ProjectC.Runtime
{
    public class MapChunk
    {
        public int ownerControllerInstanceId;
        public int instanceId;
        public GameObject planeInst;
        public MeshFilter meshFilter;
        public Vector2Int coord;

        public MapChunk(int ownerControllerInstanceId, GameObject plane, MeshFilter meshFilter, Vector2Int coord)
        {
            this.ownerControllerInstanceId = ownerControllerInstanceId;
            this.instanceId = plane.GetInstanceID();
            this.planeInst = plane;
            this.meshFilter = meshFilter;
            this.coord = coord;
        }

        public int OwnerControllerInstanceId => ownerControllerInstanceId;

        public int InstanceId => instanceId;
    }

    public class MapController : MonoBehaviour
    {
        private const int ChunkRadius = 2;

        private const int ChunkLineCount = ChunkRadius * 2 + 1;

        [SerializeField] private MapSettingsData mapSettingsData = null;

        [SerializeField] private GameObject mapChunkPf = null;

        [SerializeField, Min(0f)] private float moveSpeed = 5f;

        private GameObject planeRoot = null;

        private WorldManager worldManager = null;

        private readonly List<MapChunk> mapChunks = new();

        private void OnEnable()
        {
            worldManager = WorldManager.GetInstance();
            worldManager.MapChunkRegistered += HandleMapChunkRegistered;
        }

        private void OnDisable()
        {
            if (worldManager == null) return;

            worldManager.MapChunkRegistered -= HandleMapChunkRegistered;
        }

        private void Start()
        {
            GenerateMapChunk();
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (mapChunks.Count == 0) return;

            float moveDistance = moveSpeed * Time.deltaTime;

            for (int i = 0; i < mapChunks.Count; i++)
            {
                MoveChunk(mapChunks[i], moveDistance);
            }
        }

        /// <summary>
        /// 맵 청크 오브젝트 25개 생성
        /// </summary>
        [ContextMenu("ProjectC/MapController/GenerateMapChunk")]
        public void GenerateMapChunk()
        {
            worldManager ??= WorldManager.GetInstance();

            planeRoot = new GameObject("PlaneRoot");
            planeRoot.transform.SetParent(transform);

            // 청크를 중심 좌표 기준 5x5로 배치합니다.
            for (int z = ChunkRadius; z >= -ChunkRadius; z--)
            {
                for (int x = -ChunkRadius; x <= ChunkRadius; x++)
                {
                    Vector2Int coord = new Vector2Int(x, z);

                    // chunk 인스턴싱
                    GameObject newChunkInst = Instantiate(mapChunkPf, planeRoot.transform);
                    newChunkInst.name = $"Plane_({x}, {z})";
                    newChunkInst.transform.localPosition = new Vector3(x * mapSettingsData.ChunkSize, 0f, z * mapSettingsData.ChunkSize);

                    MeshFilter meshFilter = newChunkInst.AddComponent<MeshFilter>();
                    meshFilter.mesh = CreateLowpolyMap(coord);

                    MeshRenderer meshRenderer = newChunkInst.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = mapSettingsData.PlaneMat;

                    // chunk 소유권은 WorldManager에 등록하고, 컨트롤러는 콜백으로 목록만 동기화합니다.
                    worldManager.RegisterMapChunk(GetInstanceID(), newChunkInst, meshFilter, coord);
                }
            }
        }

        public bool TryGetMapChunk(Vector2Int coord, out MapChunk mapChunk)
        {
            mapChunk = null;

            for (int i = 0; i < mapChunks.Count; i++)
            {
                if (mapChunks[i].coord != coord) continue;

                mapChunk = mapChunks[i];
                return true;
            }

            return false;
        }

        private void MoveChunk(MapChunk mapChunk, float moveDistance)
        {
            Transform chunkTransform = mapChunk.planeInst.transform;
            chunkTransform.localPosition += Vector3.back * moveDistance;

            float chunkSize = mapSettingsData.ChunkSize;
            float backLimitZ = chunkSize * -(ChunkRadius + 1);

            // 뒤쪽 경계를 넘은 해당 청크만 앞쪽으로 순환시킵니다.
            while (chunkTransform.localPosition.z <= backLimitZ)
            {
                Vector3 localPosition = chunkTransform.localPosition;
                localPosition.z += chunkSize * ChunkLineCount;
                chunkTransform.localPosition = localPosition;

                Vector2Int newCoord = new Vector2Int(mapChunk.coord.x, mapChunk.coord.y + ChunkLineCount);

                mapChunk.planeInst.name = $"Plane_({newCoord.x}, {newCoord.y})";

                Mesh oldMesh = mapChunk.meshFilter.mesh;
                mapChunk.meshFilter.mesh = CreateLowpolyMap(newCoord);
                if (oldMesh != null)
                {
                    Destroy(oldMesh);
                }

                worldManager.UpdateMapChunkCoord(mapChunk, newCoord);
            }
        }

        private Mesh CreateLowpolyMap(Vector2Int chunkCoord)
        {
            int size = mapSettingsData.Size;
            float vertexSpacing = mapSettingsData.VertexSpacing;

            Vector3[,] points = new Vector3[size + 1, size + 1];

            float halfSize = mapSettingsData.ChunkSize * 0.5f;

            for (int z = 0; z <= size; z++)
            {
                for (int x = 0; x <= size; x++)
                {
                    float height = GetVertexHeight(x, z, chunkCoord);
                    points[x, z] = new Vector3(x * vertexSpacing - halfSize, height, z * vertexSpacing - halfSize);
                }
            }

            Vector3[] vertices = new Vector3[size * size * 6];
            int[] triangles = new int[vertices.Length];

            int index = 0;

            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector3 bottomLeft = points[x, z];
                    Vector3 bottomRight = points[x + 1, z];
                    Vector3 topLeft = points[x, z + 1];
                    Vector3 topRight = points[x + 1, z + 1];

                    bool flipDiagonal = Random.value > 0.5f;

                    if (flipDiagonal)
                    {
                        // 대각선 방향: bottomLeft -> topRight
                        vertices[index] = bottomLeft;
                        vertices[index + 1] = topLeft;
                        vertices[index + 2] = topRight;

                        vertices[index + 3] = bottomLeft;
                        vertices[index + 4] = topRight;
                        vertices[index + 5] = bottomRight;
                    }
                    else
                    {
                        // 대각선 방향: topLeft -> bottomRight
                        vertices[index] = bottomLeft;
                        vertices[index + 1] = topLeft;
                        vertices[index + 2] = bottomRight;

                        vertices[index + 3] = topLeft;
                        vertices[index + 4] = topRight;
                        vertices[index + 5] = bottomRight;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        triangles[index + i] = index + i;
                    }

                    index += 6;
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "ChunkMesh_generated";
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private float GetVertexHeight(int x, int z, Vector2Int chunkCoord)
        {
            // 월드 기준 그리드 좌표로 샘플링해서 인접 청크의 경계 높이가 이어지게 한다.
            int size = mapSettingsData.Size;
            float noiseScale = mapSettingsData.NoiseScale;
            int heightLevels = mapSettingsData.HeightLevels;

            float worldX = chunkCoord.x * size + x;
            float worldZ = chunkCoord.y * size + z;
            float noise = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale);
            float steppedNoise = Mathf.Round(noise * heightLevels) / heightLevels;

            // Perlin의 자연스러운 흐름은 유지하되 높이를 단계화해서 lowpoly 질감을 만든다.
            return (steppedNoise - 0.5f) * 2f * mapSettingsData.RandomHeight;
        }

        private void HandleMapChunkRegistered(MapChunk mapChunk)
        {
            if (mapChunk.OwnerControllerInstanceId != GetInstanceID()) return;

            mapChunks.Add(mapChunk);
        }
    }
}
