using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace CastleGenerator.Tier0
{
    // todo: prefab visualizer
    public class CellGeneratorVisualizer : MonoBehaviour
    {
        public bool PressSpaceToContinue;
        public float StepDelay; // seconds

        private CellGeneratorController _cellGenController;
        private Bounds _bounds;

        public async Task Visualize(CellGeneratorController cellGenController, Bounds bounds)
        {
            _cellGenController = cellGenController;
            _bounds = bounds;
            await UniTask.Delay((int) (StepDelay * 1000f));
            if (PressSpaceToContinue)
                await UniTask.WaitUntil(()=>Input.GetKeyDown(KeyCode.Space));
        }

        void OnDrawGizmos()
        {
            if (_cellGenController == null)
                return;

            var data = _cellGenController.Data;
            var height = data.GetLength(1);
            var width = data.GetLength(0);

            // Draw the AABB wire cube
            Gizmos.color = Color.magenta; // You can choose a different color
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);

            // Draw cells
            Vector3 startBottomLeft = _bounds.min;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    // Calculate the position for each WireCube 
                    Vector3 cubePosition = startBottomLeft + new Vector3(x + 0.5f, y + 0.5f, 0);

                    if (data[x, y] == CastleGenerator.Val1)
                    {
                        var clr = Color.gray;
                        clr.a = 0.6f;
                        Gizmos.color = clr;
                        Gizmos.DrawCube(cubePosition,
                            new Vector3(0.8f, 0.8f, 0.2f)); // You can adjust the size of the WireCube as needed
                    }
                    else if (data[x, y] == CastleGenerator.Val2)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawCube(cubePosition,
                            new Vector3(0.8f, 0.8f, 0.2f)); // You can adjust the size of the WireCube as needed
                    }
                    else if (data[x, y] == CastleGenerator.Val3)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(cubePosition,
                            new Vector3(0.8f, 0.8f, 0.2f)); // You can adjust the size of the WireCube as needed
                    }
                }
        }
    }
}