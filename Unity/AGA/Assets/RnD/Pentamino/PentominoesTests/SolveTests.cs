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
            var solutions = new Omino().Solve(null, 1);
            Assert.IsTrue(520 == solutions.Count());
        }
    }
}
