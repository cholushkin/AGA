using UnityEngine;
using System.Linq;
using GameLib.Random;
using System.Collections;
using UnityEngine.Events;
using NaughtyAttributes;

public class CellPatternChunk : MonoBehaviour
{
    [Header("Symmetry settings")]
    public bool SymmetryVertical;
    public bool SymmetryHorizontal;
    public bool SymmetryDiagonalLeft;
    public bool SymmetryDiagonalRight;
    public bool SymmetryRotationQuad;
    public bool SymmetryRotationHalf;

    [Tooltip("Ratio of enabled cells to total cells")]
    [Range(0f, 1f)]
    public float MinSaturation;
    public Vector2Int ChunkSize;
    public long Seed;
    public bool GenerateOnStart;
    public UnityEvent OnGenerate;
    BitArray _pattern;

    private IPseudoRandomNumberGenerator _rnd;


    public void Reset()
    {
        SetDefaultState();
    }


    private void Start()
    {
        if (GenerateOnStart)
            Generate();
    }


    private void OnValidate()
    {
        Generate();
    }


    public void SetDefaultState()
    {
        SymmetryVertical = true;
        SymmetryHorizontal = false;
        SymmetryDiagonalLeft = false;
        SymmetryDiagonalRight = false;
        SymmetryRotationQuad = false;
        SymmetryRotationHalf = false;

        MinSaturation = 0.6f;
        ChunkSize = new Vector2Int(8, 8);
        Seed = -1;
        GenerateOnStart = true;
    }


    [Button]
    public void Generate()
    {
        _rnd = RandomHelper.CreateRandomNumberGenerator(Seed);
        Seed = (int)_rnd.GetState().AsNumber();
        _pattern = new BitArray(ChunkSize.x * ChunkSize.y);

        while (GetSaturation() < MinSaturation)
            Enable(_rnd.Range(0, ChunkSize.x), _rnd.Range(0, ChunkSize.y));

        OnGenerate?.Invoke();
    }


    // Enable cells in the pattern based on symmetry settings
    public void Enable(int col, int row)
    {
        RotSet(col, row, true);

        int halfX = (int)Mathf.Floor(ChunkSize.x / 2.0f);
        int halfY = (int)Mathf.Floor(ChunkSize.y / 2.0f);
        bool oddSizeX = ChunkSize.x % 2 == 1;
        bool oddSizeY = ChunkSize.y % 2 == 1;

        int mCol = -(col - halfX) + halfX - (oddSizeX ? 0 : 1);
        int mRow = -(row - halfY) + halfY - (oddSizeY ? 0 : 1);

        if (SymmetryVertical)
            RotSet(mCol, row, true);
        if (SymmetryHorizontal)
            RotSet(col, mRow, true);
        if (SymmetryVertical && SymmetryHorizontal)
            RotSet(mCol, mRow, true);
    }


    // Set the value of a pixel at a given position
    public void Set(int col, int row, bool value)
    {
        int index = (col + row * ChunkSize.x);
        if (col < 0 || row < 0)
            return;
        if (col >= ChunkSize.x)
            return;
        if (row >= ChunkSize.y)
            return;
        _pattern[index] = value;
    }

    public bool Get(int col, int row)
    {
        return _pattern[col + row * ChunkSize.x];
    }


    // Set the value of pixels in a diagonal pattern
    private void DiagSet(int col, int row, bool value)
    {
        int halfX = ChunkSize.x / 2;
        int halfY = ChunkSize.y / 2;
        bool oddSizeX = ChunkSize.x % 2 == 1;
        bool oddSizeY = ChunkSize.y % 2 == 1;

        int mCol = -(col - halfX) + halfX - (oddSizeX ? 0 : 1);
        int mRow = -(row - halfY) + halfY - (oddSizeY ? 0 : 1);

        Set(col, row, value);
        if (SymmetryDiagonalRight)
            Set(mRow, mCol, value);
        if (SymmetryDiagonalLeft)
            Set(row, col, value);
        if (SymmetryDiagonalRight && SymmetryDiagonalLeft)
            Set(mCol, mRow, value);
    }


    // Set the value of pixels in a rotated pattern
    private void RotSet(int col, int row, bool value)
    {
        DiagSet(col, row, value);

        if (SymmetryRotationQuad)
        {
            int rRow = row;
            int rCol = col;

            for (int i = 0; i < 3; i++)
            {
                int tmp = rCol;
                rCol = ChunkSize.x - rRow - 1;
                rRow = tmp;

                DiagSet(rCol, rRow, value);
            }
        }

        if (SymmetryRotationHalf)
        {
            int halfX = ChunkSize.x / 2;
            int halfY = ChunkSize.y / 2;
            bool oddSizeX = ChunkSize.x % 2 == 1;
            bool oddSizeY = ChunkSize.y % 2 == 1;

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
        return (ChunkSize.x * ChunkSize.y, _pattern.Cast<bool>().Count(bit => bit));
    }
}



