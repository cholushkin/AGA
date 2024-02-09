using CastleGenerator.Tier0;
using CastleGenerator.Tier1;
using CastleGenerator.Tier2;
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
    public class CastleGenerator : MonoBehaviour
    {
        [Header("Castle")]
        // =============================================
        public bool GenerateOnStart;
        
        public const byte Val0 = 0; // empty and not used
        public const byte Val1 = 1; // initial cell generation patterns
        public const byte Val2 = 2; // flood filled value| available for polyomino spawn
        public const byte Val3 = 3; // polyomino placed

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
        private CellPattern _cellPattern;

        [Header("Tier 1")]
        // =============================================
        public CastleChunkMetaProvider CastleChunkProvider;
        public CastlePolyominoGenerator CastlePolyominoGenerator;
        public long SeedTier1;
        public LogChecker LogT1;

        private IPseudoRandomNumberGenerator _rndTier1;
        private PolyominoProvider _polyominoProvider;

        [Header("Tier 2")]
        // =============================================
        public CastleChunkGenerator ChunkGenerator;
        public long SeedTier2;
        public LogChecker LogT2;
        private IPseudoRandomNumberGenerator _rndTier2;
        
        

        async void Start()
        {
            if (GenerateOnStart)
            {
                LogT0.Print(LogChecker.Level.Verbose, $"Auto start generating castle on '{transform.GetDebugName()}'");
                await GenerateCastle();
            }
        }

        [Button("Tier 0 + 1 + 2")]
        public async UniTask GenerateCastle()
        {
            LogT0.Print(LogChecker.Level.Normal, $">>>>> [method]CastleGeneratorController.GenerateCastle");
            await Tier0Task();
            await Tier1Task();
            await Tier2Task();
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
                _cellPattern = Instantiate(CellPatternProvider.GetRandom(), CellPatternOutput);
                _cellPattern.transform.localPosition = Vector3.zero;
                _cellPattern.Init(_rndTier0, LogT0); // Propagate seed
                fillIterationCurrentPat = 0;

                do
                {
                    LogT0.Print(LogChecker.Level.Verbose, $"T0. Fill attempt iteration {fillIterationCurrentPat}.");
                    await CellGenerator.Generate(_cellPattern);
                    if (VisualizeTier0)
                        await CellGeneratorVisualizer.Visualize(CellGenerator, _cellPattern.Bounds);

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
            (_rndTier1, SeedTier1) = SetTierSeed(SeedTier1);
            LogT1.Print(LogChecker.Level.Normal, $"T1 Seed: {SeedTier1}");
            
            CastleChunkProvider.Init();
             _polyominoProvider = new PolyominoProvider(CastleChunkProvider.Metas);
            CastlePolyominoGenerator.Init(CellGenerator, _polyominoProvider, _rndTier1, LogT1);
            await CastlePolyominoGenerator.Generate();
        }
        
        [Button()]
        public async UniTask Tier2Task()
        {
            (_rndTier2, SeedTier2) = SetTierSeed(SeedTier2);
            LogT2.Print(LogChecker.Level.Normal, $"T2 Seed: {SeedTier2}");
            ChunkGenerator.Init(_polyominoProvider, CastlePolyominoGenerator, _cellPattern.Bounds.min, _rndTier2, LogT2);
            await ChunkGenerator.Generate();
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