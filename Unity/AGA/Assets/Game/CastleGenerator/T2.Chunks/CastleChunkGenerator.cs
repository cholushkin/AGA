using System.Linq;
using System.Threading.Tasks;
using CastleGenerator.Tier1;
using GameLib.Log;
using GameLib.Random;
using TowerGenerator;
using UnityEngine;


namespace CastleGenerator.Tier2
{
    public class CastleChunkGenerator : MonoBehaviour
    {
        private PolyominoProvider _polyominoProvider;
        private CastlePolyominoGenerator _polyominoGenerator;
        private IPseudoRandomNumberGenerator _rnd;
        private LogChecker _log;
        private Vector3 _bottomLeftCorner;
        
        public void Init(PolyominoProvider polyominoProvider, CastlePolyominoGenerator polyominoGenerator,
            Vector3 bottomLeftCorner, IPseudoRandomNumberGenerator rnd, LogChecker log)
        {
            _rnd = rnd;
            _bottomLeftCorner = bottomLeftCorner;
            _polyominoProvider = polyominoProvider;
            _polyominoGenerator = polyominoGenerator;
            _log = log;
        }

        public async Task Generate()
        {
            foreach (var p in _polyominoGenerator.GetPlacedPolyominos())
            {
                // Get random model with probability
                var modelProbs = p.polyomino.Models.Select(x => x.Probability).ToArray();
                var modelIndex = _rnd.SpawnEvent(modelProbs);
                var model = p.polyomino.Models[modelIndex];
                
                // Spawn model
                CreateCastleChunk(model.Meta, _rnd.GetState().AsNumber(), transform, new Vector3(p.pos.x, p.pos.y, 0));
            }
        }

        private void CreateCastleChunk(CastleChunkMeta meta, long seed, Transform parent, Vector3 position)
        {
            var pathInResources = ChunkImportSourceHelper.GetPathInResources(meta.ImportSource.ChunksOutputPath);
            var chunkPrefab = (GameObject)Resources.Load(pathInResources + "/" + meta.ChunkName);
            chunkPrefab.SetActive(false);
            var chunk = Object.Instantiate(chunkPrefab);

            chunk.name = $"{chunkPrefab.name}{position}";
            chunk.transform.position = _bottomLeftCorner + position + new Vector3(0.5f, 0.5f, 0f);
            chunk.transform.SetParent(parent);

            var baseChunkController = chunk.GetComponent<ChunkControllerBase>();
            baseChunkController.Seed = seed;
            baseChunkController.Init();
            chunk.SetActive(true);
            baseChunkController.SetConfiguration();
        }
    }
}