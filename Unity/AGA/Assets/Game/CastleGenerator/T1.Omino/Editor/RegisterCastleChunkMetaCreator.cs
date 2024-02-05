using TowerGenerator;
using TowerGenerator.ChunkImporter;
using TowerGenerator.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace CastleGenerator.Tier1.Editor
{
    [InitializeOnLoad]
    public static class RegisterCastleChunkMetaCreator
    {
        static RegisterCastleChunkMetaCreator()
        {
            MetaTypeRegistry.RegisterMetaType(new MetaTypeRegistry.MetaTypeEntry {Name = "CastleChunkMeta", Creator = new CastleChunkMetaCreator()});
        }
    }
    
    public class CastleChunkMetaCreator : IMetaCreator
    {
        public MetaBase Create(GameObject chunkObject, ChunkImportSource importSource, ChunkCooker.ChunkImportState importState)
        {
            var chunkController = chunkObject.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController, "chunk must have a ChunkControllerBase");
            string assetPathAndName = importSource.MetasOutputPath + "/" + importState.ChunkName + ".castlemeta.asset";
            var metaAsset = AssetDatabase.LoadAssetAtPath<CastleChunkMeta>(assetPathAndName); // Try to load existing asset first to keep references to the asset alive
            var isCreated = false;
            if (metaAsset == null)
            {
                metaAsset = ScriptableObject.CreateInstance<CastleChunkMeta>();
                isCreated = true;
            }
            
            chunkController.Meta = metaAsset;
            metaAsset.ChunkName = importState.ChunkName;
            metaAsset.ChunkControllerType = importState.ChunkControllerType;
            metaAsset.Generation = importState.Generation;
            metaAsset.TagSet = importState.ChunkTagSet;
            metaAsset.ChunkMargin = 1f; // todo: FbxCommand ChunkMargin(0)
            metaAsset.AABB = chunkController.CalculateDimensionAABB().size;
            metaAsset.ImportSource = importSource;
            metaAsset.Shape = chunkObject.GetComponent<CastleChunk>().Shape;
            
            if(isCreated)
                AssetDatabase.CreateAsset(metaAsset, assetPathAndName);
            AssetDatabase.SaveAssets();
            Debug.Log($"Import castle meta: {metaAsset}");
            return metaAsset;
        }
    }
}