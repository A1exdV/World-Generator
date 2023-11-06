using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(menuName = "Generator/Chunk config", fileName = "new Chunk config")]
    public class ChunkConfigSo : ScriptableObject
    {
        [field:SerializeField,Range(0,1)]public float ObstacleChance { get; private set; }
        [field:SerializeField,Range(0,1)]public float DecorativeChance { get; private set; }
        
        [field:SerializeField]public Transform[] ObstaclePrefabArray { get; private set; }
        [field:SerializeField]public Transform[] DecorativePrefabArray { get; private set; }
    }
}
