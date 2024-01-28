using System.Collections;
using System.Collections.Generic;
using GameLib.Random;
using UnityEngine;

namespace CastleGenerator.Tier0
{
    public class CellChunkRndParamMutator : MonoBehaviour
    {
        public bool RandomizeSeed;
        public float SaturationAbsDelta;

        public void MutateParameters(CellChunkRnd cellChunk, IPseudoRandomNumberGenerator rnd)
        {
            if(RandomizeSeed)
                cellChunk.SetSeed(rnd.ValueInt());
            if (SaturationAbsDelta != 0.0f)
            {
                var delta = rnd.Range(0f, SaturationAbsDelta * 2f) - SaturationAbsDelta;
                cellChunk.MinSaturation += delta;
            }
        }
    }
}