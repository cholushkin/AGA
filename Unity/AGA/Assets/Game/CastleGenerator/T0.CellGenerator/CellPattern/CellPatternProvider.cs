using GameLib.Log;
using GameLib.Random;
using UnityEngine;


namespace CastleGenerator.Tier0
{
    // todo: Provider variant with each cell pattern probabilities
    // todo: Provider variant which populate from specified resource folder and generation parameter
    public class CellPatternProvider : MonoBehaviour
    {
        public CellPattern[] CellPatternsPool;
        private IPseudoRandomNumberGenerator _rnd;
        private LogChecker _log;
        
        public void Init(IPseudoRandomNumberGenerator rnd, LogChecker log)
        {
            _log = log;
            _rnd = rnd;
            _log.Print(LogChecker.Level.Normal, "CellPatternProvider.Init", transform);
            Populate();
        }
        
        void Populate()
        {
            _log.Print(LogChecker.Level.Verbose, $"CellPatternProvider uses static array of patterns CellPatternsPool. {CellPatternsPool.Length} items.");
        }

        public CellPattern GetRandom()
        {
            return _rnd.FromArray(CellPatternsPool);
        }
    }
}