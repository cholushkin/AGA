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
        
        void OnDrawGizmos()
        {
            // Assuming your script is attached to a game object, get the position
            Vector3 position = transform.position;

            // Calculate the half extents of the AABB
            Vector3 halfExtents = new Vector3(Width, Height, 1) * 0.5f;

            // Draw the AABB wire cube
            Gizmos.color = Color.white; // You can choose a different color
            Gizmos.DrawWireCube(position, halfExtents * 2f);
            
            // Draw cells
            Gizmos.color = Color.yellow; // You can choose a different color
            Vector3 startBottomLeft = position - halfExtents + Vector3.one * 0.5f;
            
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                // Calculate the position for each WireCube 
                Vector3 cubePosition = startBottomLeft + new Vector3(x, y, 0);

                // Draw the WireCube at the calculated position
                Gizmos.DrawWireCube(cubePosition, new Vector3(1f, 1f, 0.1f));
            }
        }
    }
}