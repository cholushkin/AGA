using GameLib.Random;
using UnityEngine;


namespace CastleGenerator.Tier0
{
    // todo: Provider variant with each cell pattern probabilities
    // todo: Provider variant which populate from specified resource folder and generation parameter
    public class CellPatternProvider : MonoBehaviour
    {
        public long Seed;
        public CellPattern[] CellPatternsPool;

        private IPseudoRandomNumberGenerator _rnd;
        
        public void Init(long seed)
        {
            Debug.Log("CellPatternProvider Init", transform);
            
            SetSeed(seed);
            Populate();
        }
        
        void Populate()
        {
            Debug.Log($"CellPatternProvider uses static array of patterns CellPatternsPool. {CellPatternsPool.Length} items.");
        }

        public CellPattern GetRandom()
        {
            return _rnd.FromArray(CellPatternsPool);
        }

        public void SetSeed(long seed)
        {
            _rnd = RandomHelper.CreateRandomNumberGenerator(seed);
            Seed = (int) _rnd.GetState().AsNumber();
            Debug.Log($"CellPatternProvider set seed: {Seed}", transform);
        }
    }
}