using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace WorldGeneration
{

    public class ChunkManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Height and width of one chunk in world dimension. It is also responsible for the size of the grid inside the chunk")] 
        private int chunkSize = 10;
        [SerializeField, Tooltip("How many chunks will be displayed around target object.")] 
        private int loadingRadiusInChunks = 5;
        [SerializeField] private Chunk chunkPrefab;

        [SerializeField, Tooltip("Config for start chunk with player.")] 
        private ChunkConfigSo zeroConfig;
        [SerializeField] private ChunkConfigSo defaultConfig;

        
        [SerializeField, Tooltip("Object that will be used as center of rendering zone.")] 
        private Transform target;
        
        /// <summary>
        /// Save of all created chunks data in this session.
        /// </summary>
        private Dictionary<Vector2, ChunkData> _createdChunkDataDictionary = new Dictionary<Vector2, ChunkData>();

        /// <summary>
        /// Chunks that are in rendering zone.
        /// </summary>
        private Dictionary<Chunk, Vector2> _loadedChunkDictionary = new Dictionary<Chunk, Vector2>();
        
        /// <summary>
        /// Index of a chunk with target object.
        /// </summary>
        private Vector2Int _targetChunkIndex;

        private void Awake()
        {
            _targetChunkIndex = WorldPositionToChunksIndex(target.position);
            InstantiateChunkObjects();
        }

        private void Update()
        {
            var newTargetChunkIndex = WorldPositionToChunksIndex(target.position);
            if (_targetChunkIndex != newTargetChunkIndex)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                _targetChunkIndex = newTargetChunkIndex;
                UpdateLoadingZone(_targetChunkIndex);
                stopWatch.Stop();
                Debug.Log($"Loading zone update took {stopWatch.ElapsedMilliseconds} ms.");
            }
        }

        /// <summary>
        /// This method instantiates chunk objects around the target within a specified loading radius,
        /// populating them with chunk data.
        /// </summary>
        private void InstantiateChunkObjects()
        {
            var xMin = _targetChunkIndex.x - loadingRadiusInChunks;
            var xMax = _targetChunkIndex.x + loadingRadiusInChunks;
            var yMin = _targetChunkIndex.y - loadingRadiusInChunks;
            var yMax = _targetChunkIndex.y + loadingRadiusInChunks;
            
            
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    var chunk = Instantiate(chunkPrefab,transform);
                    var index = new Vector2Int(x, y);
                    
                    if (index == Vector2Int.zero)
                        chunk.UpdateChunkData(CreateChunkData(index,zeroConfig));
                    else
                        chunk.UpdateChunkData(GetChunkData(index));
                    _loadedChunkDictionary.Add(chunk,index);
                    chunk.transform.position = new Vector3(x, 0, y) * chunkSize;
                    chunk.name = $"Chunk {index.x}, {index.y}";
                }
            }
        }

        /// <summary>
        /// This method updates the loading zone of chunks around the given center.
        /// It overwrites chunks that have gone beyond the rendering to chunks that have just entered the visibility zone.
        /// </summary>
        private void UpdateLoadingZone(Vector2Int center)
        {
            var xMin = center.x - loadingRadiusInChunks;
            var xMax = center.x + loadingRadiusInChunks;
            var yMin = center.y - loadingRadiusInChunks;
            var yMax = center.y + loadingRadiusInChunks;
            
            List<Chunk> chunkOutOfRenderList = GetChunksOutOfRender(xMin,xMax,yMin,yMax);
            
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    if (!_loadedChunkDictionary.ContainsValue(new Vector2(x, y)))
                    {
                        _loadedChunkDictionary[chunkOutOfRenderList[^1]] = new Vector2(x, y);
                        chunkOutOfRenderList[^1].UpdateChunkData(GetChunkData(new Vector2Int(x,y)));
                        chunkOutOfRenderList[^1].transform.position = new Vector3(x, 0, y) * chunkSize;
                        chunkOutOfRenderList[^1].name = $"Chunk {x}, {y}";
                        chunkOutOfRenderList.Remove(chunkOutOfRenderList[^1]);
                    }
                }
            }
        }
        
        /// <summary>
        /// This method returns a list of chunks that are outside the specified render boundaries.
        /// </summary>
        private List<Chunk> GetChunksOutOfRender(int xMin, int  xMax, int  yMin, int  yMax)
        {
            List<Chunk> chunkList = new List<Chunk>();
            
            foreach (var chunkPair in _loadedChunkDictionary)
            {
                if (chunkPair.Value.x > xMax || chunkPair.Value.x < xMin || chunkPair.Value.y > yMax || chunkPair.Value.y < yMin)
                {
                    chunkList.Add(chunkPair.Key);
                }
            }
            return chunkList;
        }

        /// <summary>
        /// This method retrieves the chunk data for a given index.
        /// If the data is not found, it either creates a new chunk data or loads it from a saved dictionary if found.
        /// </summary>
        private ChunkData GetChunkData(Vector2Int index)
        {
            if (_createdChunkDataDictionary.ContainsKey(index))
            {
                //found
                return GetChunkFromSave(index);
            }
            //absent
            return CreateChunkData(index, defaultConfig);
        }

        /// <summary>
        /// This method creates a new chunk data with the provided configuration and associates it with the specified index in the dictionary.
        /// </summary>
        private ChunkData CreateChunkData(Vector2Int index,ChunkConfigSo config)
        {
            ChunkData newChunkData = new ChunkData(config,chunkSize);
            _createdChunkDataDictionary.Add(index, newChunkData);
            return newChunkData;
        }

        /// <summary>
        /// This method retrieves a chunk data from the saved dictionary based on the given index.
        /// </summary>
        private ChunkData GetChunkFromSave(Vector2Int index)
        {
            ChunkData newChunkData = _createdChunkDataDictionary[index];
            return newChunkData;
        }
        
        /// <summary>
        /// This method converts a world position into chunk coordinate.
        /// </summary>
        /// <param name="position">World position</param>
        /// <returns>Corresponding chunk index</returns>
        private Vector2Int WorldPositionToChunksIndex(Vector3 position)
        {
            return new Vector2Int((int)(position.x/chunkSize),(int)(position.z/chunkSize));
        }
    }
}
