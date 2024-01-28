using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CastleGenerator.Tier0
{
    // todo: defferent fill patterns 

    public class CellChunkFill : CellChunkBase
    {
        public override void Generate()
        {
            var size = GetSize();
            _data = new byte[size.width, size.height];
            for (int x = 0; x < size.width; ++x)
                for (int y = 0; y < size.height; ++y)
                    SetCell(x, y, 1);

            base.Generate();
        }
    }
}