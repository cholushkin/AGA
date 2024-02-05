using CastleGenerator.Tier0;
using CastleGenerator.Tier1;
using Cysharp.Threading.Tasks;
using GameLib.Alg;
using GameLib.Log;
using GameLib.Random;
using NaughtyAttributes;
using TowerGenerator;
using UnityEditor;
using UnityEngine;

namespace CastleGenerator
{
    public class CastleGeneratorController : MonoBehaviour
    {
        [Header("Castle")]
        // =============================================
        public bool GenerateOnStart;

        [Header("Tier 0")]
        // =============================================
        public CellGeneratorController CellGenerator;

        public CellPatternProvider CellPatternProvider;
        public CellGeneratorVisualizer CellGeneratorVisualizer;
        public int MaxFillIteration; // How many iterations to take before change to the next CellPattern
        public int MaxPickIteration;
        public Transform CellPatternOutput;
        public bool VisualizeTier0;
        public long SeedTier0;
        public LogChecker LogT0;

        private IPseudoRandomNumberGenerator _rndTier0;
        private int _cellPatternPickIteration;
        private int _cellFillIteration;

        [Header("Tier 1")]
        // =============================================
        public MetaProvider CastleChunkProvider;

        public CastleChunkGenerator CastleChunkGenerator;
        public long SeedTier1;
        public LogChecker LogT1;

        private IPseudoRandomNumberGenerator _rndTier1;

        async void Start()
        {
            if (GenerateOnStart)
            {
                LogT0.Print(LogChecker.Level.Verbose, $"Auto start generating castle on '{transform.GetDebugName()}'");
                await GenerateCastle();
            }
        }

        [Button()]
        public async UniTask GenerateCastle()
        {
            LogT0.Print(LogChecker.Level.Normal, $">>>>> [method]CastleGeneratorController.GenerateCastle");
            await Tier0Task();
        }


        [Button()]
        public async UniTask Tier0Task()
        {
            bool finishGeneration = false;
            int fillIterationCurrentPat = 0;

            _cellFillIteration = 0;
            _cellPatternPickIteration = 0;

            (_rndTier0, SeedTier0) = SetTierSeed(SeedTier0);
            LogT0.Print(LogChecker.Level.Normal, $"T0 Seed: {SeedTier0}");

            CellPatternProvider.Init(_rndTier0, LogT0); // Propagate seed
            CellGenerator.Init(LogT0);

            while (!finishGeneration && _cellPatternPickIteration < MaxPickIteration)
            {
                LogT0.Print(LogChecker.Level.Verbose, $"T0. Cell pattern pick iteration {_cellPatternPickIteration}.");
                CellPatternOutput.DestroyChildren();
                await UniTask.Yield();
                var cellPattern = Instantiate(CellPatternProvider.GetRandom(), CellPatternOutput);
                cellPattern.transform.localPosition = Vector3.zero;
                cellPattern.Init(_rndTier0, LogT0); // Propagate seed
                fillIterationCurrentPat = 0;

                do
                {
                    LogT0.Print(LogChecker.Level.Verbose, $"T0. Fill attempt iteration {fillIterationCurrentPat}.");
                    await CellGenerator.Generate(cellPattern);
                    if (VisualizeTier0)
                        await CellGeneratorVisualizer.Visualize(CellGenerator, cellPattern.Bounds);

                    LogT0.Print(LogChecker.Level.Verbose, $"Cell generator status: {CellGenerator.Status}.");
                    ++fillIterationCurrentPat;
                    ++_cellFillIteration;
                } while (CellGenerator.Status != CellGeneratorController.ResultStatus.Success &&
                         fillIterationCurrentPat < MaxFillIteration);

                finishGeneration = (CellGenerator.Status == CellGeneratorController.ResultStatus.Success);
                ++_cellPatternPickIteration;
            }

            LogT0.Print(LogChecker.Level.Normal,
                $"T0 finished with status '{CellGenerator.Status}'. Took Pattern pick iterations = {_cellPatternPickIteration}; Fill iterations = {_cellFillIteration};");
        }

        [Button()]
        public async UniTask Tier1Task()
        {
            var pieceDescriptions = new PieceDescriptions(CastleChunkProvider.Metas);
            CastleChunkGenerator.Init(CellGenerator,pieceDescriptions, LogT1);
            await CastleChunkGenerator.Generate();
        }

        private (IPseudoRandomNumberGenerator, long) SetTierSeed(long seed)
        {
            var rnd = RandomHelper.CreateRandomNumberGenerator(seed);
            var newSeed = rnd.GetState().AsNumber();
            return (rnd, newSeed);
        }





        void OnDrawGizmos()
        {
            if (LogT0.Gizmos)
                Handles.Label(transform.position + Vector3.down,
                    $"T0.Seed : {SeedTier0}\nT0.Fills iter: {_cellFillIteration}");
        }
    }
}