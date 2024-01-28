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
            Assert.IsNotNull(_data, "CellChunkClone.Generate must be called after Original CellChunkBase" );
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
            int rows = original.GetLength(0); // y
            int cols = original.GetLength(1); // x
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
        
        void OnDrawGizmos()
        {
            // Assuming your script is attached to a game object, get the position
            Vector3 position = transform.position;

            // Calculate the half extents of the AABB
            Vector3 halfExtents = new Vector3(Width, Height, 1) * 0.5f;

            // Draw the AABB wire cube
            Gizmos.color = Color.magenta; // You can choose a different color
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
                Gizmos.DrawWireCube(cubePosition, new Vector3(1f, 1f, 0.1f)); // You can adjust the size of the WireCube as needed
            }
        }
    }
}