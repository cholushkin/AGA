using System.Collections.Generic;
using System.Linq;
using PentominoesLib;
using UnityEngine;

namespace PentominoesApp
{
    using Solution = List<Placement>;

    class Test : MonoBehaviour
    {
        void Awake()
        {
            print("asda");    
            Main();
        }

        void Main()
        {
            var solutions = Pentominoes.Solve();
            var solutionsCount = solutions.Aggregate(0, (acc, solution) =>
            {
                DrawSolution(solution);
                return acc + 1;
            });
            Debug.Log($"Number of solutions found: {solutionsCount}");
        }

        private void DrawSolution(Solution solution)
        {
            foreach (var line in Pentominoes.FormatSolution(solution))
            {
                Debug.Log(line);
            }
            Debug.Log(new string('-', 80));
        }
    }
}
