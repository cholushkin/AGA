using NaughtyAttributes;
using TowerGenerator;
using UnityEngine;

namespace CastleGenerator.Tier1
{
    public class CastleChunkMetaProvider : MetaProviderGeneric<CastleChunkMeta>
    {
        [Button()]
        private void DbgPrintShapes()
        {
            int i = 0;
            foreach (var castleMeta in Metas)
                Debug.Log($"{i++}. {castleMeta.Shape}");
        }
    }
}