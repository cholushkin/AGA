using System;
using System.Collections.Generic;
using System.Linq;
using PentominoesLib;

namespace PentominoesApp
{
    using Solution = List<Placement>;

    class Program
    {
        static void Main(string[] args)
        {
            var solutions = Pentominoes.Solve();
            var solutionsCount = solutions.Aggregate(0, (acc, solution) =>
            {
                DrawSolution(solution);
                return acc + 1;
            });
            Console.WriteLine($"Number of solutions found: {solutionsCount}");
        }

        private static void DrawSolution(Solution solution)
        {
            foreach (var line in Pentominoes.FormatSolution(solution))
            {
                Console.WriteLine(line);
            }
            Console.WriteLine(new string('-', 80));
        }
    }
}
