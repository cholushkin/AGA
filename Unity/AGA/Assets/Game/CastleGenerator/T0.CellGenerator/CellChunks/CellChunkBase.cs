using UnityEngine;
using UnityEngine.Events;

namespace CastleGenerator.Tier0
{
    public interface ICellChunk
    {
        (int width, int height) GetSize();
        byte GetCell(int x, int y);
        void SetCell(int x, int y, byte val);
        byte[,] GetData();
        void Generate();
    }
    
    // todo: support mask. User draw cells which must be ampty (negative mask)
    // todo: positive mask

    public class CellChunkBase : MonoBehaviour, ICellChunk
    {
        protected byte[,] _data;
        
        public UnityEvent OnGenerate;
        public int Width;
        public int Height;
        
        public (int width, int height) GetSize()
        {
            return (Width, Height);
        }

        public byte GetCell(int x, int y)
        {
            return _data[x, y];
        }

        public void SetCell(int x, int y, byte val)
        {
            if (x < 0 || y < 0)
                return;
            var chunkSize = GetSize();
            if (x >= chunkSize.width || y >= chunkSize.height)
                return;
            _data[x, y] = val;
        }

        public byte[,] GetData()
        {
            return _data;
        }

        public virtual void Generate()
        {
            OnGenerate?.Invoke();
        }
    }
}