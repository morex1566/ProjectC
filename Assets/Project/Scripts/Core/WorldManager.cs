using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectC.Runtime
{
    /// <summary>
    /// 월드 런타임 인스턴스의 소유권과 조회 컨테이너를 관리합니다.
    /// </summary>
    public class WorldManager : MonoBehaviourSingleton<WorldManager>
    {
        public static WorldManagerSettingsData worldManagerSettingsData;

        private readonly List<MapChunk> mapChunks = new();

        private readonly List<RailSegment> railSegments = new();

        public event Action<MapChunk> MapChunkRegistered;

        public event Action<RailSegment> RailRegistered;

        /// <summary>
        /// 월드 매니저 인스턴스를 보장합니다.
        /// </summary>
        public static void Init()
        {
            WorldManager worldManager = GetInstance();
            worldManagerSettingsData = Resources.Load<WorldManagerSettingsData>("SO_WorldManagerSettings");
            worldManager.ClearRuntimeState();
        }

        public MapChunk RegisterMapChunk(int ownerControllerInstanceId, GameObject planeInst, MeshFilter meshFilter, Vector2Int coord)
        {
            MapChunk mapChunk = new MapChunk(ownerControllerInstanceId, planeInst, meshFilter, coord);

            mapChunks.Add(mapChunk);

            // 등록 이벤트를 통해 컨트롤러 목록과 WorldManager 컨테이너를 동기화합니다.
            MapChunkRegistered?.Invoke(mapChunk);

            return mapChunk;
        }

        public RailSegment RegisterRail(int ownerControllerInstanceId, GameObject railInst, Vector2Int coord)
        {
            RailSegment railSegment = new RailSegment(ownerControllerInstanceId, railInst, coord);

            railSegments.Add(railSegment);

            // 등록 이벤트를 통해 컨트롤러 목록과 WorldManager 컨테이너를 동기화합니다.
            RailRegistered?.Invoke(railSegment);

            return railSegment;
        }

        public bool TryGetMapChunk(int instanceId, out MapChunk mapChunk)
        {
            mapChunk = null;

            for (int i = 0; i < mapChunks.Count; i++)
            {
                if (mapChunks[i].InstanceId != instanceId) continue;

                mapChunk = mapChunks[i];
                return true;
            }

            return false;
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

        public void UpdateMapChunkCoord(MapChunk mapChunk, Vector2Int newCoord)
        {
            // 청크 인스턴스는 재사용하고 좌표만 현재 논리 위치로 갱신합니다.
            mapChunk.coord = newCoord;
        }

        public bool TryGetRail(int instanceId, out RailSegment railSegment)
        {
            railSegment = null;

            for (int i = 0; i < railSegments.Count; i++)
            {
                if (railSegments[i].InstanceId != instanceId) continue;

                railSegment = railSegments[i];
                return true;
            }

            return false;
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

        public void UpdateRailCoord(RailSegment railSegment, Vector2Int newCoord)
        {
            // 레일 인스턴스는 재사용하고 좌표만 현재 논리 위치로 갱신합니다.
            railSegment.coord = newCoord;
        }

        private void ClearRuntimeState()
        {
            mapChunks.Clear();
            railSegments.Clear();
        }
    }
}
