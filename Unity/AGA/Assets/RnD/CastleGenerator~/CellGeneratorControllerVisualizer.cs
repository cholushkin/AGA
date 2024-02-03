using NaughtyAttributes;
using UnityEngine;

namespace CastleGenerator
{
    public class CellGeneratorControllerVisualizer : MonoBehaviour
    {
        public GameObject PrefabCell;
        public GameObject PrefabIslandCell;
        public CellGeneratorController CastleController;

        [Button]
        public void Visualize()
        {
            if (!enabled)
                return;

            if (CastleController.Status != CellGeneratorController.ResultStatus.Success)
            {
                Debug.Log($"Castle generator error {CastleController.Status}");
                return;
            }

            var currentVisualization = transform.Find("Visualization");
            if (currentVisualization != null)
            {
                DestroyImmediate(currentVisualization.gameObject);
                currentVisualization = new GameObject("Visualization").transform;
                currentVisualization.transform.parent = transform;
            }

            int columns = CastleController.Data.GetLength(0);
            int rows = CastleController.Data.GetLength(1);

            print($"{columns} {rows}");
            var bottomLeft = CastleController.Bounds.min;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (CastleController.Data[x, y] == 2)
                    {
                        Instantiate(
                            PrefabCell,
                            bottomLeft + new Vector3(x + 0.5f, y + 0.5f, 0),
                            Quaternion.identity,
                            currentVisualization);
                    }

                    if (CastleController.Data[x, y] == 1)
                    {
                        Instantiate(
                            PrefabIslandCell,
                            bottomLeft + new Vector3(x + 0.5f, y + 0.5f, 0),
                            Quaternion.identity,
                            currentVisualization);
                    }
                }
            }
        }
    }
}