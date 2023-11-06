using UnityEngine;

namespace WorldGeneration
{
    public class Chunk : MonoBehaviour
    {
        public ChunkData ChunkData;
        private void Start()
        {
            InstantiateStaticObjects();
        }

        private void InstantiateStaticObjects()
        {
            foreach (var obstacle in ChunkData.StaticObjectsList)
            {
                var objectTransform = Instantiate(obstacle.Value, transform);
                Vector3 objectPosition = new Vector3(obstacle.Key.x, 0, obstacle.Key.y);
                objectTransform.localPosition = objectPosition;
            }
        }
    }
}
