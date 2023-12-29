using System.Diagnostics;
using System.Linq;
using PentominoesLib;
using UnityEngine.Assertions;

namespace PentominoesTests
{
    public class SolveTests
    {
        public void FindsCorrectNumberOfSolutions()
        {
            var solutions = Pentominoes.Solve(null);
            Assert.IsTrue(520 == solutions.Count());
        }
    }
}
