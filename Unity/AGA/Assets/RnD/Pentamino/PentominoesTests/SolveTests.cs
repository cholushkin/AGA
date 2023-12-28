using System.Diagnostics;
using System.Linq;
using PentominoesLib;

namespace PentominoesTests
{
    public class SolveTests
    {
        public void FindsCorrectNumberOfSolutions()
        {
            var solutions = Pentominoes.Solve();
            Debug.Assert(65 == solutions.Count());
        }
    }
}
