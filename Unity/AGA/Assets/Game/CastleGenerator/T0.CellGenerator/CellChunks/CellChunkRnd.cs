using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.PlayerLoop;

namespace CastleGenerator.Tier0
{
    public class CellChunkRnd : CellChunkBase
    {
        [Tooltip("Ratio of enabled cells to total cells")] [Range(0f, 1f)]
        public float MinSaturation;
        public CellChunkRndParamMutator Mutator;
        private IPseudoRandomNumberGenerator _rnd;
        
        [Header("Symmetry settings")] public bool SymmetryVertical;
        public bool SymmetryHorizontal;
        public bool SymmetryDiagonalLeft;
        public bool SymmetryDiagonalRight;
        public bool SymmetryRotationQuad;
        public bool SymmetryRotationHalf;

        public void SetRnd(IPseudoRandomNumberGenerator rnd)
        {
            _rnd = rnd;
        }

        public override void Generate()
        {
            if(Mutator != null)
                Mutator.MutateParameters(this, _rnd);
            
            _data = new byte[Width, Height];

            while (GetSaturation() < MinSaturation)
                Enable(_rnd.Range(0, Width), _rnd.Range(0, Height));

            base.Generate();
        }
        
         // Enable cells in the pattern based on symmetry settings
        private void Enable(int col, int row)
        {
            RotSet(col, row, 1);
            var chunkSize = GetSize();
            int halfX = (int) Mathf.Floor(chunkSize.width / 2.0f);
            int halfY = (int) Mathf.Floor(chunkSize.height / 2.0f);
            bool oddSizeX = chunkSize.width % 2 == 1;
            bool oddSizeY = chunkSize.height % 2 == 1;

            int mCol = -(col - halfX) + halfX - (oddSizeX ? 0 : 1);
            int mRow = -(row - halfY) + halfY - (oddSizeY ? 0 : 1);

            if (SymmetryVertical)
                RotSet(mCol, row, 1);
            if (SymmetryHorizontal)
                RotSet(col, mRow, 1);
            if (SymmetryVertical && SymmetryHorizontal)
                RotSet(mCol, mRow, 1);
        }

        // Set the value of pixels in a diagonal pattern
        private void DiagSet(int col, int row, byte value)
        {
            var chunkSize = GetSize();
            int halfX = chunkSize.width / 2;
            int halfY = chunkSize.height / 2;
            bool oddSizeX = chunkSize.width % 2 == 1;
            bool oddSizeY = chunkSize.height % 2 == 1;

            int mCol = -(col - halfX) + halfX - (oddSizeX ? 0 : 1);
            int mRow = -(row - halfY) + halfY - (oddSizeY ? 0 : 1);

            SetCell(col, row, value);
            if (SymmetryDiagonalRight)
                SetCell(mRow, mCol, value);
            if (SymmetryDiagonalLeft)
                SetCell(row, col, value);
            if (SymmetryDiagonalRight && SymmetryDiagonalLeft)
                SetCell(mCol, mRow, value);
        }


        // Set the value of pixels in a rotated pattern
        private void RotSet(int col, int row, byte value)
        {
            var chunkSize = GetSize();
            DiagSet(col, row, value);

            if (SymmetryRotationQuad)
            {
                int rRow = row;
                int rCol = col;

                for (int i = 0; i < 3; i++)
                {
                    int tmp = rCol;
                    rCol = chunkSize.width - rRow - 1;
                    rRow = tmp;

                    DiagSet(rCol, rRow, value);
                }
            }

            if (SymmetryRotationHalf)
            {
                int halfX = chunkSize.width / 2;
                int halfY = chunkSize.height / 2;
                bool oddSizeX = chunkSize.width % 2 == 1;
                bool oddSizeY = chunkSize.height % 2 == 1;

                int mCol = -(col - halfX) + halfX - (oddSizeX ? 0 : 1);
                int mRow = -(row - halfY) + halfY - (oddSizeY ? 0 : 1);

                this.DiagSet(mCol, mRow, value);
            }
        }

        public float GetSaturation()
        {
            var num = GetCellsNumber();
            return num.set / (float) num.all;
        }

        public (int all, int set) GetCellsNumber()
        {
            var chunkSize = GetSize();
            return (chunkSize.width * chunkSize.height, _data.Cast<byte>().Count(value => value > 0));
        }
    }
}
