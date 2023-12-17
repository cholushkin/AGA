using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameLib.Alg;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace CastleGenerator
{
    public class CastleGeneratorIterator : MonoBehaviour
    {
        public GameObject[] CellPatterns;
        public CellGeneratorController CellGenerator;
        public CellGeneratorValidator CellGeneratorValidator;
        public bool GenerateOnAwake;
        public int MaxIterations;
        public int IterationVisualizationDelay; // milliseconds
        public UnityEvent OnGenerate;
        public Transform CellPatternParent;

        public long Seed;


        public async void Awake()
        {
            if (GenerateOnAwake)
                await Generate();
        }

        [Button()]
        public async UniTask Generate()
        {
            Debug.Log($"Generating castle");
            var iteration = 0;
            bool finishGeneration = false;

            while (!finishGeneration)
            {
                if (iteration > 0)
                    await Task.Delay(IterationVisualizationDelay);
                finishGeneration = await Iterate(iteration);
                iteration++;

                OnGenerate?.Invoke();
                finishGeneration = finishGeneration || iteration >= MaxIterations;
            }

            Debug.Log($"Iterations={iteration} Cells={CellGeneratorValidator.CellsCount}");

        }

        private async UniTask<bool> Iterate(int iteration)
        {
            Debug.Log($">>>>> Iteration {iteration}");

            // Cell Pattern chunks
            {
                CellPatternParent.DestroyChildren();
                await UniTask.DelayFrame(1);

                var prefab = CellPatterns[Random.Range(0, CellPatterns.Length)];
                Instantiate(prefab, CellPatternParent);
                await UniTask.DelayFrame(1);
            }

            // Generate cell pattern
            {
                // Mutate parameters
                CellGenerator.MutateParameters();


                CellGenerator.Generate();
                if (CellGenerator.Status != CellGeneratorController.ResultStatus.Success)
                {
                    Debug.Log($"Iteration {iteration}: CellGenerator failed - {CellGenerator.Status}");
                    return false;
                }
            }

            CellGeneratorValidator.Validate();
            if (CellGeneratorValidator.Status != CellGeneratorValidator.ValidateStatus.Pass)
            {
                Debug.Log($"Iteration {iteration}: CellGeneratorValidator failed - {CellGeneratorValidator.Status}");
                return false;
            }

            return true;
        }
    }
}