using UnityEngine;
using UnityEngine.Events;


namespace CastleGenerator
{

    [ExecuteInEditMode]
    public class CellPatternChunkClone : CellPatternChunkBase
    {
        public CellPatternChunk Original;
        public bool MirrorLeftToRight;
        public UnityEvent OnGenerate;

        public override Vector2Int GetChunkSize()
        {
            return Original.GetChunkSize();
        }

        public override void Generate()
        {
            // copy from original
            if (MirrorLeftToRight)
                _pattern = MirrorArrayLeftToRight(Original._pattern);
            else
                _pattern = (byte[,])Original._pattern.Clone();
            OnGenerate?.Invoke();
        }


        private byte[,] MirrorArrayUpDown(byte[,] original)
        {
            int rows = original.GetLength(0);
            int cols = original.GetLength(1);
            byte[,] mirroredArray = new byte[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mirroredArray[i, j] = original[i, cols - 1 - j];
            return mirroredArray;
        }

        private byte[,] MirrorArrayLeftToRight(byte[,] original)
        {
            int rows = original.GetLength(0);
            int cols = original.GetLength(1);
            byte[,] mirroredArray = new byte[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mirroredArray[i, j] = original[rows - 1 - i, j];
            return mirroredArray;
        }
    }
}