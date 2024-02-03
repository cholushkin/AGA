using System.Collections;
using UnityEngine;
using UnityEngine.Events;


namespace CastleGenerator
{

	public abstract class CellPatternChunkBase : MonoBehaviour
    {
        internal byte[,] _pattern;

        public abstract Vector2Int GetChunkSize();

        // Set the value of a pixel at a given position
        protected void Set(int col_x, int row_y, byte value)
        {
            int index = (col_x + row_y * GetChunkSize().x);
            if (col_x < 0 || row_y < 0)
                return;
            if (col_x >= GetChunkSize().x)
                return;
            if (row_y >= GetChunkSize().y)
                return;
            _pattern[col_x, row_y] = value;
        }

        public byte Get(int col_x, int row_y)
        {
            return _pattern[col_x, row_y];
        }

        public abstract void Generate();

    }
}