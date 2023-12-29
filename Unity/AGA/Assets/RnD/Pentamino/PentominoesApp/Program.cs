using System;
using System.Linq;
using System.Collections.Generic;
using PentominoesLib;
using DlxLib;
using UnityEngine;

namespace PentominoesApp
{
    class Program : MonoBehaviour
    {
	    void Awake()
	    {
		    Main();
	    }

        static void Main()
        {
            var solutions = Pentominoes.Solve(OnSolutionFound);
            Debug.Log($"Total number of solutions found: {solutions.Count()}");
            Debug.Log($"Number of unique solutions found: {UniqueSolutions.Count()}");
        }

        private static void OnSolutionFound(IEnumerable<Placement> rows, Solution solution, int solutionIndex)
        {
            if (IsSolutionUnique(rows, solution))
            {
                UniqueSolutions.Add(solution);
                var board = FormatBoard(rows, solution);
                UniqueJoinedBoards.Add(string.Join("|", board));
                DrawSolution(board);
            }
        }

        private static List<Solution> UniqueSolutions = new List<Solution>();
        private static List<string> UniqueJoinedBoards = new List<string>();

        private static bool IsSolutionUnique(IEnumerable<Placement> rows, Solution solution)
        {
            var board1 = FormatBoard(rows, solution);
            var board2 = StringManipulations.RotateStrings(board1);
            var board3 = StringManipulations.RotateStrings(board2);
            var board4 = StringManipulations.RotateStrings(board3);
            var board5 = StringManipulations.ReflectStrings(board1);
            var board6 = StringManipulations.ReflectStrings(board2);
            var board7 = StringManipulations.ReflectStrings(board3);
            var board8 = StringManipulations.ReflectStrings(board4);
            var joinedBoards = new[] {
                string.Join("|", board1),
                string.Join("|", board2),
                string.Join("|", board3),
                string.Join("|", board4),
                string.Join("|", board5),
                string.Join("|", board6),
                string.Join("|", board7),
                string.Join("|", board8)
            };
            return UniqueJoinedBoards.Intersect(joinedBoards).Count() == 0;
        }

        private static void DrawSolution(IEnumerable<string> board)
        {
            foreach (var line in board)
            {
                Debug.Log(line);
            }
            Debug.Log(new string('-', 80));
        }

        private static IEnumerable<string> FormatBoard(IEnumerable<Placement> rows, Solution solution)
        {
	        var rowsList = rows.ToList();
	        var seed = new List<(int x, int y, string label)>
	        {
		        (3, 3, " "),
		        (3, 4, " "),
		        (4, 3, " "),
		        (4, 4, " ")
	        };

	        var cells = solution.RowIndexes.Aggregate(seed, (accOuter, rowIndex) =>
	        {
		        var placement = rowsList[rowIndex];
		        return placement.Variation.Coords.Aggregate(accOuter, (accInner, coords) =>
		        {
			        var x = placement.Location.X + coords.X;
			        var y = placement.Location.Y + coords.Y;
			        accInner.Add((x, y, placement.Piece.Label));
			        return accInner;
		        });
	        });

	        return Enumerable.Range(0, 8)
		        .Select(y =>
		        {
			        var row = Enumerable.Range(0, 8).Select(x => cells.First(t => t.x == x && t.y == y));
			        return string.Join("", row.Select(t => t.label));
		        });
        }

    }
}
