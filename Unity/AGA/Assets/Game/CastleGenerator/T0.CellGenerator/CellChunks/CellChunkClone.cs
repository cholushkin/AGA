using System.Collections;
using System.Collections.Generic;
using CastleGenerator.Tier0;
using UnityEngine;
using UnityEngine.Assertions;

namespace CastleGenerator
{
    class CellChunkClone : CellChunkBase
    {
        public enum Mirror
        {
            None,
            Vertical,
            Horizontal,
        }
        public CellChunkBase Original;
        public Mirror MirrorType;


        public override void Generate()
        {
            Assert.IsNotNull(Original.GetData(), "CellChunkClone.Generate must be called after Original CellChunkBase" );
            _data = MirrorType switch
            {
                Mirror.Vertical => MirrorArrayLeftToRight(Original.GetData()),
                Mirror.Horizontal => MirrorArrayUpDown(Original.GetData()),
                _ => (byte[,]) Original.GetData().Clone()
            };
            base.Generate();
        }

        private byte[,] MirrorArrayUpDown(byte[,] original)
        {
            int width = original.GetLength(0); // x|width|cols
            int height = original.GetLength(1); // y|height|rows
            byte[,] mirroredArray = new byte[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    mirroredArray[i, j] = original[i, height - 1 - j];
            return mirroredArray;
        }

        private byte[,] MirrorArrayLeftToRight(byte[,] original)
        {
            int width = original.GetLength(0); // x|width|cols
            int height = original.GetLength(1); // y|height|rows
            byte[,] mirroredArray = new byte[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    mirroredArray[i, j] = original[width - 1 - i, j];
            return mirroredArray;
        }
    }
}