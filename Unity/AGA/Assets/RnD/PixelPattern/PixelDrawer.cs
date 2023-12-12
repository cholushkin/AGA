using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NaughtyAttributes;
using GameLib.Random;

public class PixelDrawer : MonoBehaviour
{
    public Vector2Int DoodleSize = new Vector2Int (8, 8);
    
    [Range(0,1)]
    public float MinSaturation = 0.55f;

    [System.Serializable]
    public struct PixelSettings
    {
        public int RemoveIfXAndLessNeighbours; // 3

    }

    [System.Serializable]
    public struct SymetrySettings
    {
        public bool symmetryVertical;
        public bool symmetryHorizontal;
        public bool symmetryDiagonalLeft;
        public bool symmetryDiagonalRight;
        public bool symmetryRotationQuad;
        public bool symmetryRotationHalf;
    }

    public long Seed;
    public SymetrySettings Symetry;
    public PixelSettings PixSettings;
    IPseudoRandomNumberGenerator Rnd;


    private Texture2D texture;
    private Color[] colors;
    private RawImage rawImage;

    [Button()]
    void Start()
    {
        Rnd = RandomHelper.CreateRandomNumberGenerator(Seed);
        Seed = (int)Rnd.GetState().AsNumber();

        texture = new Texture2D(DoodleSize.x, DoodleSize.y);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        colors = new Color[DoodleSize.x * DoodleSize.y];

        GetComponent<RectTransform>().sizeDelta = new Vector2(DoodleSize.x * 10, DoodleSize.y * 10);;
        rawImage = GetComponent<RawImage>();
        rawImage.texture = texture;

        DrawPixels();
    }

    private void OnValidate()
    {
        Start();
    }

    void DrawPixels()
    {
        PixelSticker ps = new PixelSticker(DoodleSize, Symetry, PixSettings);

        while (ps.GetSaturation() < MinSaturation)
        {
            int randomCol = Mathf.FloorToInt(Rnd.Range(0f, DoodleSize.x));
            int randomRow = Mathf.FloorToInt(Rnd.Range(0f, DoodleSize.y));

            ps.Enable(randomCol, randomRow);
        }



        for (int row = 0; row < texture.height; row++)
        {
            for (int col = 0; col < texture.width; col++)
            {
                colors[row * texture.width + col] = ps.Get(col, row) ? Color.black : Color.white;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
    }

    class PixelSticker
    {
        private bool[] pattern;
        private Vector2Int size;
        private SymetrySettings symetrySettings;
        private PixelSettings pixSettings;

        public PixelSticker(Vector2Int size, SymetrySettings settings, PixelSettings pixSettings)
        {
            this.pattern = new bool[size.x * size.y];
            this.size = size;
            this.symetrySettings = settings;
            this.pixSettings = pixSettings;
        }


        // Set the value of a pixel at a given position
        public void Set(int col, int row, bool value)
        {
            int index = (int)(col + row * size.x);
            if (col < 0 || row < 0)
                return;
            if(col >= size.x)
                return;
            if (row >= size.y)
                return;
            this.pattern[index] = value;
        }

        public bool Get(int col, int row)
        {
            return pattern[col + row * (int)size.x];
        }

        // Set the value of pixels in a diagonal pattern
        private void DiagSet(int col, int row, bool value)
        {
            int halfX = (int)size.x / 2;
            int halfY = (int)size.y / 2;
            bool oddSizeX = (int)size.x % 2 == 1;
            bool oddSizeY = (int)size.y % 2 == 1;

            int mCol = -(col - halfX) + halfX - (oddSizeX ? 0 : 1);
            int mRow = -(row - halfY) + halfY - (oddSizeY ? 0 : 1);

            this.Set(col, row, value);
            if (symetrySettings.symmetryDiagonalRight)
            {
                this.Set(mRow, mCol, value);
            }
            if (symetrySettings.symmetryDiagonalLeft)
            {
                this.Set(row, col, value);
            }
            if (symetrySettings.symmetryDiagonalRight && symetrySettings.symmetryDiagonalLeft)
            {
                this.Set(mCol, mRow, value);
            }
        }


        // Set the value of pixels in a rotated pattern
        private void RotSet(int col, int row, bool value)
        {
            this.DiagSet(col, row, value);

            if (symetrySettings.symmetryRotationQuad)
            {
                int rRow = row;
                int rCol = col;

                for (int i = 0; i < 3; i++)
                {
                    int tmp = rCol;
                    rCol = (int)size.x - rRow - 1;
                    rRow = tmp;

                    this.DiagSet(rCol, rRow, value);
                }
            }

            if (symetrySettings.symmetryRotationHalf)
            {
                int halfX = (int)size.x / 2;
                int halfY = (int)size.y / 2;
                bool oddSizeX = (int)size.x % 2 == 1;
                bool oddSizeY = (int)size.y % 2 == 1;

                int mCol = -(col - halfX) + halfX - (oddSizeX ? 0 : 1);
                int mRow = -(row - halfY) + halfY - (oddSizeY ? 0 : 1);

                this.DiagSet(mCol, mRow, value);
            }
        }



        // Enable pixels in the pattern based on symmetry settings
        public void Enable(int col, int row)
        {
            this.RotSet(col, row, true);

            int halfX = (int)Mathf.Floor(size.x / 2.0f);
            int halfY = (int)Mathf.Floor(size.y / 2.0f);
            bool oddSizeX = (int)size.x % 2 == 1;
            bool oddSizeY = (int)size.y % 2 == 1;

            int mCol = -(col - halfX) + halfX - (oddSizeX ? 0 : 1);
            int mRow = -(row - halfY) + halfY - (oddSizeY ? 0 : 1);

            if (symetrySettings.symmetryVertical)
            {
                this.RotSet(mCol, row, true);
            }
            if (symetrySettings.symmetryHorizontal)
            {
                this.RotSet(col, mRow, true);
            }
            if (symetrySettings.symmetryVertical && symetrySettings.symmetryHorizontal)
            {
                this.RotSet(mCol, mRow, true);
            }
        }


        // Get the value of a pixel at a given position
        public bool Value(int col, int row)
        {
            return this.pattern[col + row * (int)size.x];
        }

        // Calculate the saturation of the pattern (ratio of enabled pixels to total pixels)
        public float GetSaturation()
        {
            return this.pattern.Count(p => p) / (float)(size.x * size.y);
        }

    }
}



                                 