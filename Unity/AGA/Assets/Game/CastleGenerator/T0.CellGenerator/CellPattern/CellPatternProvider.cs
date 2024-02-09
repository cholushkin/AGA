using System.Collections.Generic;
using System.Linq;
using GameLib.Log;
using GameLib.Random;
using UnityEngine;


namespace CastleGenerator.Tier0
{
    // todo: Provider variant with each cell pattern probabilities
    public class CellPatternProvider : MonoBehaviour
    {
        public List<CellPattern> CellPatterns;
        public List<CellPatternProviderPopulatorBase> Populators; 
        private IPseudoRandomNumberGenerator _rnd;
        private LogChecker _log;
        
        public void Init(IPseudoRandomNumberGenerator rnd, LogChecker log)
        {
            _log = log;
            _rnd = rnd;
            _log.Print(LogChecker.Level.Normal, "CellPatternProvider.Init", transform);
            foreach (var populator in Populators)
                populator.Populate(this);
        }
        
        public void Populate(List<CellPattern> patterns)
        {
            CellPatterns.AddRange(patterns);
            CellPatterns = CellPatterns.Distinct().ToList();
        }

        public CellPattern GetRandom()
        {
            return _rnd.FromList(CellPatterns);
        }
    }
}