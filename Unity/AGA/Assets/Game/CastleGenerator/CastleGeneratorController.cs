using CastleGenerator.Tier0;
using Cysharp.Threading.Tasks;
using GameLib.Alg;
using GameLib.Log;
using NaughtyAttributes;
using UnityEditor.VersionControl;
using UnityEngine;
using CellGeneratorController = CastleGenerator.Tier0.CellGeneratorController;

public class CastleGeneratorController : MonoBehaviour
{
    [Header("Tier 0")] public CellGeneratorController CellGenerator;
    public CellPatternProvider CellPatternProvider;
    public CellGeneratorVisualizer CellGeneratorVisualizer;
    public int MaxFillIteration; // How many iterations to take before change to the next CellPattern
    public int MaxPickIteration; 
    public Transform CellPatternOutput;
    public bool VisualizeTier0;
    public long SeedTier0;


    [Header("Castle")] public LogChecker Log;

    private int _cellPatternIteration;
    

    [Button()]
    public async void Generate()
    {
        bool finishGeneration = false;
        int cellPatternIteration = 0;
        int fillIteration = 0;

        CellPatternProvider.Init(SeedTier0); // Propagate seed
        
        while (!finishGeneration && cellPatternIteration < MaxPickIteration)
        {
            Debug.Log($"Tier 0. Cell pattern pick iteration {cellPatternIteration}.");
            CellPatternOutput.DestroyChildren();
            await UniTask.Yield();
            var cellPattern = Instantiate(CellPatternProvider.GetRandom(), CellPatternOutput);
            //await UniTask.Yield();
            cellPattern.Init(CellPatternProvider); // Propagate seed
            fillIteration = 0;

            do
            {
                Debug.Log($"Tier 0. Fill attempt iteration {fillIteration}.");
                await CellGenerator.Generate(cellPattern);
                if (VisualizeTier0)
                    await CellGeneratorVisualizer.Visualize(CellGenerator, cellPattern.Bounds);

                Debug.Log($"Cell generator status: {CellGenerator.Status}.");
                ++fillIteration;
            } while (CellGenerator.Status != CellGeneratorController.ResultStatus.Success &&
                     fillIteration < MaxFillIteration);

            finishGeneration = (CellGenerator.Status == CellGeneratorController.ResultStatus.Success);
            ++cellPatternIteration;
        }
        Debug.Log($"Tier 0 finished: {CellGenerator.Status}. Took 'pattern iterations' = {cellPatternIteration}, 'last fill iterations' = {fillIteration}");
    }
}