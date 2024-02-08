using System.Collections;
using System.Collections.Generic;
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
        
        public void Init( PolyominoProvider polyominoProvider, CastlePolyominoGenerator polyominoGenerator, IPseudoRandomNumberGenerator rnd, LogChecker log)
        {
            _rnd = rnd;
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
                ChunkFactory.CreateChunkRnd(model.Meta, _rnd.GetState().AsNumber(), transform, new Vector3(p.pos.x, p.pos.y, 0));
                Debug.Log($">>>{p.pos}");
                
            }

            
        }
    }

    
}
