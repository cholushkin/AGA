using UnityEngine;
using System.Linq;
using GameLib.Random;
using System.Collections;
using UnityEngine.Events;
using NaughtyAttributes;


namespace CastleGenerator
{

    [ExecuteInEditMode]
    public class CellPatternChunkClone : CellPatternChunkBase
    {
        public CellPatternChunk Original;
        public bool Mirror;

        public override void Generate()
        {
	        OnGenerate?.Invoke();
        }

        public override void Set(int col, int row, bool value)
        {
	        throw new System.NotImplementedException();
        }

        public override bool Get(int col, int row)
        {
	        return Original.Get(col, row);
        }
    }
}