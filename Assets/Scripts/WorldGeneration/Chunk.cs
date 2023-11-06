using UnityEngine;

namespace WorldGeneration
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Transform floor;
        
        /// <summary>
        /// The data associated with this chunk.
        /// </summary>
        private ChunkData _chunkData;

        /// <summary>
        /// Update the chunk's data and regenerate its content.
        /// </summary>
        public void UpdateChunkData(ChunkData chunkData)
        {
            _chunkData = chunkData;
            DestroyChildes();
            InstantiateStaticObjects();
        }

        /// <summary>
        /// Destroy child game objects within the chunk, excluding the floor.
        /// </summary>
        private void DestroyChildes()
        {
            foreach (Transform child in transform)
            {
                if(child == floor) continue;
                
                Destroy(child.gameObject);
            }
        }
        
        /// <summary>
        /// Instantiate static objects within the chunk based on the provided data.
        /// </summary>
        private void InstantiateStaticObjects()
        {
            foreach (var staticObject in _chunkData.StaticObjectsDictionary)
            {
                var objectTransform = Instantiate(staticObject.Value.TransformObject, transform);
                var objectPosition = new Vector3(staticObject.Key.x, 0, staticObject.Key.y);
                objectTransform.localPosition = objectPosition;
                objectTransform.localEulerAngles = new Vector3(0, staticObject.Value.Rotation, 0);
            }
        }
    }
}
