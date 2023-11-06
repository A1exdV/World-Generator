using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace WorldGeneration
{

    public class ChunkManager : MonoBehaviour
    {
        
        [FormerlySerializedAs("chunkScale")]
        [SerializeField, Tooltip("Height and width of one chunk in world dimension. " +
                                 "It is also responsible for the size of the grid inside the chunk")] 
        private int chunkSize = 10;
        [SerializeField, Tooltip("How many chunks will be displayed around target object.")] 
        private int loadingRadiusInChunks = 5;
        [SerializeField] private Chunk chunkPrefab;

        [SerializeField] private ChunkConfigSo zeroConfig;
        [SerializeField] private ChunkConfigSo defaultConfig;

        [SerializeField] private Transform target;

        /// <summary>
        /// All chunks data generated in this session. Something like local save. 
        /// </summary>
        private Dictionary<Vector2, ChunkData> _createdChunkDataDictionary = new Dictionary<Vector2, ChunkData>();
        /// <summary>
        /// Chunks that are in loading radius and are shown in game
        /// </summary>
        private Dictionary<Vector2, Chunk> _loadedChunkDictionary = new Dictionary<Vector2, Chunk>();

        /// <summary>
        /// Current chunk index with target
        /// </summary>
        private Vector2Int _targetChunkIndex;

        private void Awake()
        {
            _targetChunkIndex = WorldPositionToChunksIndex(target.position);
            CreateChunk(new Vector2Int(0,0),zeroConfig);
            UpdateLoadingZone(Vector2Int.zero);
        }

        private void Update()
        {
            var newTargetChunkIndex = WorldPositionToChunksIndex(target.position);
            if (_targetChunkIndex != newTargetChunkIndex)
            {
                _targetChunkIndex = newTargetChunkIndex;
                UpdateLoadingZone(_targetChunkIndex);
            }
        }

        /// <summary>
        /// Updates the loaded chunks inside the loading zone. Deletes the chunks that came out of it and loads the chunks that just got into the zone.
        /// </summary>
        /// <param name="center">The center of the radius of the loading zone</param>
        private void UpdateLoadingZone(Vector2Int center)
        {
            var xMin = center.x - loadingRadiusInChunks;
            var xMax = center.x + loadingRadiusInChunks;
            var yMin = center.y - loadingRadiusInChunks;
            var yMax = center.y + loadingRadiusInChunks;
            
            UnloadChunks(xMin,xMax,yMin,yMax);
            
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    if (!_loadedChunkDictionary.ContainsKey(new Vector2(x, y)))
                    {
                        LoadChunk(new Vector2Int(x,y));
                    }
                }
            }
        }

        /// <summary>
        /// Checks chunks for exiting from the required zone.
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMin"></param>
        /// <param name="yMax"></param>
        private void UnloadChunks(int xMin, int  xMax, int  yMin, int  yMax)
        {
            List<Vector2> indexToDestroyList = new List<Vector2>();
            
            foreach (var chunkPair in _loadedChunkDictionary)
            {
                if (chunkPair.Key.x > xMax || chunkPair.Key.x < xMin || chunkPair.Key.y > yMax || chunkPair.Key.y < yMin)
                {
                    indexToDestroyList.Add(chunkPair.Key);
                }
            }
            foreach (var index in indexToDestroyList)
            {
                Destroy(_loadedChunkDictionary[index].gameObject);
                _loadedChunkDictionary.Remove(index);
            }
        }

        /// <summary>
        /// Load or create a new chunk, if it was not created earlier
        /// </summary>
        /// <param name="index"></param>
        private void LoadChunk(Vector2Int index)
        {
            if (_createdChunkDataDictionary.TryGetValue(index, out var chunkJson))
            {
                //found
                GetChunkFromSave(index);
            }
            else
            {
                //absent
                CreateChunk(index, defaultConfig);
            }
        }

        /// <summary>
        /// Creates a new chunk
        /// </summary>
        /// <param name="index"></param>
        private void CreateChunk(Vector2Int index,ChunkConfigSo config)
        {
            Vector3 worldPosition = new Vector3(index.x, 0, index.y) * chunkSize;
            var chunk = Instantiate(chunkPrefab,worldPosition, Quaternion.identity,transform);
            chunk.ChunkData = new ChunkData(config,chunkSize);
            chunk.name = $"Chunk {index.x}, {index.y}";
            _createdChunkDataDictionary.Add(index, chunk.ChunkData);
            _loadedChunkDictionary.Add(index, chunk);
        }

        /// <summary>
        /// Get already created chunk from saved dictionary 
        /// </summary>
        /// <param name="index"></param>
        private void GetChunkFromSave(Vector2Int index)
        {
            Vector3 worldPosition = new Vector3(index.x, 0, index.y) * chunkSize;
            var chunk = Instantiate(chunkPrefab, worldPosition, Quaternion.identity,transform);
            chunk.ChunkData = _createdChunkDataDictionary[index];
            chunk.name = $"Chunk {index.x}, {index.y}";
            _loadedChunkDictionary.Add(index, chunk);
        }

        /// <summary>
        /// Convert json to chunk data
        /// </summary>
        /// <param name="json">json</param>
        /// <returns>chunk class</returns>
        private Chunk JsonToChunk(string json)
        {
            return JsonConvert.DeserializeObject<Chunk>(json);
        }

        /// <summary>
        /// Convert chunk data to json
        /// </summary>
        /// <param name="chunk">chunk class</param>
        /// <returns>json</returns>
        private string ChunkToJson(Chunk chunk)
        {
            return JsonConvert.SerializeObject(chunk,Formatting.None,new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        /// <summary>
        /// Convert world dimension coordinates into chunks indexes. 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector2Int WorldPositionToChunksIndex(Vector3 position)
        {
            return new Vector2Int((int)(position.x/chunkSize),(int)(position.z/chunkSize));
        }
    }
}
