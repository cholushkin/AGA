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

        public async Task Visualize(CellGeneratorController cellGenController)
        {
            await UniTask.Delay((int) (StepDelay * 1000f));
        }
    }
}