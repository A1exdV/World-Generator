using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    public class ChunkData
    {
        public Dictionary<Vector2Int, Transform> StaticObjectsList { get; private set; }
        
        public ChunkData(ChunkConfigSo config, int chunkSize)
        {
            StaticObjectsList = new Dictionary<Vector2Int, Transform>();
            
            AddObjects(config.ObstaclePrefabArray, config.ObstacleChance, chunkSize);
        }

        private void AddObjects(Transform[] objectArray, float chance, int chunkSize)
        {
            if (chance == 0) return;
            for (var x = 1; x < chunkSize; x++)
            {
                for (var y = 1; y < chunkSize; y++)
                {
                    if (Random.Range(0, 1f) <= chance)
                    {
                        Vector2Int position = new Vector2Int(x-(chunkSize/2), y-(chunkSize/2));
                        Transform transformObject = objectArray[Random.Range(0, objectArray.Length)];
                        StaticObjectsList.Add(position, transformObject);
                    }
                }
            }
        }
    }
}