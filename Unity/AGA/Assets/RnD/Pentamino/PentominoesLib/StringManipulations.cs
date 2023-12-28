using System.Linq;
using System.Collections.Generic;

namespace PentominoesLib
{
    public static class StringManipulations
    {
        public static List<string> RotateStrings(List<string> strings)
        {
            var reversedStrings = ReflectStrings(strings);
            var w = reversedStrings[0].Length;
            var h = reversedStrings.Count;
            var xs = Enumerable.Range(0, w);
            var ys = Enumerable.Range(0, h);
            return xs
                .Select(x => new string(ys.Select(y => reversedStrings[y][x]).ToArray()))
                .ToList();
        }

        public static List<string> ReflectStrings(List<string> strings)
        {
            return strings
                .Select(s => new string(s.ToCharArray().Reverse().ToArray()))
                .ToList();
        }
    }
}
