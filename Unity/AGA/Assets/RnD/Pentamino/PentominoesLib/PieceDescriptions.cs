using System.Collections.Generic;

namespace PentominoesLib
{
    public static class PieceDescriptions
    {
        public static List<PieceDescription> AllPieceDescriptions = new List<PieceDescription>
        {
            new PieceDescription("F", new List<string>
            {
                " XX",
                "XX ",
                " X "
            }),
            new PieceDescription("I", new List<string>
            {
                "X",
                "X",
                "X",
                "X",
                "X"
            }),
            new PieceDescription("L", new List<string>
            {
                "X ",
                "X ",
                "X ",
                "XX"
            }),
            new PieceDescription("P", new List<string>
            {
                "XX",
                "XX",
                "X "
            }),
            new PieceDescription("N", new List<string>
            {
                " X",
                "XX",
                "X ",
                "X "
            }),
            new PieceDescription("T", new List<string>
            {
                "XXX",
                " X ",
                " X "
            }),
            new PieceDescription("U", new List<string>
            {
                "X X",
                "XXX"
            }),
            new PieceDescription("V", new List<string>
            {
                "X  ",
                "X  ",
                "XXX"
            }),
            new PieceDescription("W", new List<string>
            {
                "X  ",
                "XX ",
                " XX"
            }),
            new PieceDescription("X", new List<string>
            {
                " X ",
                "XXX",
                " X "
            }),
            new PieceDescription("Y", new List<string>
            {
                " X",
                "XX",
                " X",
                " X"
            }),
            new PieceDescription("Z", new List<string>
            {
                "XX ",
                " X ",
                " XX"
            })
        };
    }
}
