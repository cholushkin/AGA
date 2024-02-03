using GameLib.Random;
using UnityEngine;

namespace CastleGenerator.Tier0
{
    public class CellChunkRndParamMutator : MonoBehaviour
    {
        [Tooltip("Plus-minus delta")]
        public float SaturationAbsDelta;

        public void MutateParameters(CellChunkRnd cellChunk, IPseudoRandomNumberGenerator rnd)
        {
            if (SaturationAbsDelta != 0.0f)
            {
                var delta = rnd.Range(0f, SaturationAbsDelta * 2f) - SaturationAbsDelta;
                cellChunk.MinSaturation += delta;
                cellChunk.MinSaturation = Mathf.Clamp(cellChunk.MinSaturation, 0f, 0.9f);
            }
        }
    }
}