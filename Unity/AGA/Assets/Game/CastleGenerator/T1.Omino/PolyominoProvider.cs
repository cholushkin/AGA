using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CastleGenerator.Tier1
{
    // Polyomino: https://en.wikipedia.org/wiki/Polyomino
    public class Polyomino  // Shaped figure
    {
        public List<Vector2Int> ShapeCells;
        public List<CastleChunkModel> Models;
        public string Shape;
        public float Probability; // Chance to spawn polyomino in the castle
        
        public Polyomino(string shape, float probability)
        {
            ShapeCells = CreateCoordinates(shape);
            Models = new List<CastleChunkModel>();
            Shape = shape;
            Probability = probability;
        }
        
        public void AddModel(CastleChunkMeta meta)
        {
            Models.Add(new CastleChunkModel(meta, 1f));
        }
        
        // Iterate over each character in the input shape, filters out non-'#' characters, and creates a list of
        // Vector2Int instances representing the coordinates of the '#' characters in the shape.
        // shape.Split('n'): This splits the input shape string into an array of lines using the newline character 'n'.
        // Each element in the array represents a line in the original shape string.
        // SelectMany((line, y) => line.Select((character, x) => (character, x, y))): This uses SelectMany to flatten
        // the nested structure. For each line (line) and its corresponding index (y), it then uses Select to iterate
        // over each character (character) and its index (x) within the line. The result is a sequence
        // of tuples (character, x, y) representing the character, its x-coordinate, and its y-coordinate.
        // Where(item => item.character == '#'): This filters the sequence of tuples to include only those where
        // the character is '#'. This effectively filters out all non-'#' characters.
        // Select(item => new Vector2Int(item.x, item.y)): This maps each tuple to a new Vector2Int instance, using
        // the x and y coordinates from the tuple.
        // .ToList(): Finally, the result is converted to a List<Vector2Int>.
        private List<Vector2Int> CreateCoordinates(string shape)
        {
            return shape.Split('n')
                .SelectMany((line, y) => line.Select((character, x) => (character, x, y)))
                .Where(item => item.character == '#')
                .Select(item => new Vector2Int(item.x, item.y))
                .ToList();
        }
    }

    public class CastleChunkModel
    {
        public CastleChunkModel(CastleChunkMeta meta, float probability)
        {
            Meta = meta;
            Probability = probability;
        }
        
        public CastleChunkMeta Meta;
        public float Probability; // Chance to spawn model within one polyomino
    }

    public class PolyominoProvider
    {
        public List<Polyomino> Polyominos;
        public PolyominoProvider(List<CastleChunkMeta> metas)
        {
            Polyominos = new List<Polyomino>(128);
            foreach (var meta in metas)
            {
                var p = Polyominos.FirstOrDefault(x => x.Shape == meta.Shape);
                if (p == null)
                {
                    p = new Polyomino(meta.Shape, 1f);
                    Polyominos.Add(p);
                }
                p.AddModel(meta);
            }
        }
    }
}
