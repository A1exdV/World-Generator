﻿using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    public class ChunkData
    {
        public Dictionary<Vector2Int, ObjectData> StaticObjectsDictionary { get; private set; }
        
        /// <summary>
        /// Constructor for ChunkData.
        /// Initializes StaticObjectsDictionary and adds objects to it based on the provided configuration.
        /// </summary>
        /// <param name="config">Configuration data for the chunk</param>
        /// <param name="chunkSize">Size of the chunk</param>
        public ChunkData(ChunkConfigSo config, int chunkSize)
        {
            StaticObjectsDictionary = new Dictionary<Vector2Int, ObjectData>();
            
            AddObjects(config.ObstaclePrefabArray, config.ObstacleChance, chunkSize);
            AddObjects(config.DecorativePrefabArray, config.DecorativeChance, chunkSize);
        }

        /// <summary>
        /// Adds static objects to StaticObjectsDictionary based on the given parameters.
        /// </summary>
        /// <param name="objectArray">Array of static objects to choose from</param>
        /// <param name="chance">Chance of adding an object at a specific position</param>
        /// <param name="chunkSize">Size of the chunk grid</param>
        private void AddObjects(Transform[] objectArray, float chance, int chunkSize)
        {
            if (chance == 0) return;
            for (var x = 1; x < chunkSize; x++)
            {
                for (var y = 1; y < chunkSize; y++)
                {
                    Vector2Int position = new Vector2Int(x-(chunkSize/2), y-(chunkSize/2));
                    if(StaticObjectsDictionary.ContainsKey(position)) continue;
                    
                    if (Random.Range(0, 1f) <= chance)
                    {
                        Transform transformObject = objectArray[Random.Range(0, objectArray.Length)];
                        float rotation = GetRandomRotation();
                        StaticObjectsDictionary.Add(position, new ObjectData(transformObject,rotation));
                    }
                }
            }
        }

        private float GetRandomRotation()
        {
            int direction = Random.Range(0, 5);
            return 90 * direction;
        }
    }

    public class ObjectData
    {
        public Transform TransformObject;
        public float Rotation;

        public ObjectData(Transform transformObject, float rotation)
        {
            TransformObject = transformObject;
            Rotation = rotation;
        }
    }
}