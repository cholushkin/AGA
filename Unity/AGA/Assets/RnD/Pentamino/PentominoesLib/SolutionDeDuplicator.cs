using System.Collections.Generic;
using System.Linq;

namespace PentominoesLib
{
    public static class SolutionDeDuplicator
    {
        public static bool SolutionIsUnique(
            List<Placement> placements,
            List<string> uniqueBoards)
        {
            var board1 = Pentominoes.FormatSolution(placements);
            var board2 = StringManipulations.RotateStrings(board1);
            var board3 = StringManipulations.RotateStrings(board2);
            var board4 = StringManipulations.RotateStrings(board3);
            var board5 = StringManipulations.ReflectStrings(board1);
            var board6 = StringManipulations.ReflectStrings(board2);
            var board7 = StringManipulations.ReflectStrings(board3);
            var board8 = StringManipulations.ReflectStrings(board4);
            var boards = new[] {
                string.Join("|", board1),
                string.Join("|", board2),
                string.Join("|", board3),
                string.Join("|", board4),
                string.Join("|", board5),
                string.Join("|", board6),
                string.Join("|", board7),
                string.Join("|", board8)
            };
            return !boards.Any(uniqueBoards.Contains);
        }
    }
}