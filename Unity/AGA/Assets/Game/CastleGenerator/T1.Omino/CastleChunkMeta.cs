using TowerGenerator;
using UnityEditor;
using UnityEngine;

namespace CastleGenerator.Tier1
{
    public class CastleChunkMeta : MetaBase
    {
        public static MetaBase Cook(GameObject chunkObject)
        {
            Debug.Log("Cooking CastleChunkMeta");
            return null;
        }
        public string Shape;
    }
}
