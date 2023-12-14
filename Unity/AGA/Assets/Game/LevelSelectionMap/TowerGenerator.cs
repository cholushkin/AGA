using GameLib.Alg;
using GameLib.Random;
using UnityEngine;

namespace LevelSelection
{
    public class TowerGenerator : Singleton<TowerGenerator>
    {
        interface IGeneratorStrategy
        {
            bool GenerateNextRow(Grid grid, IPseudoRandomNumberGenerator rnd, int y);
        }

        class GenericGenerator : IGeneratorStrategy
        {
            public bool GenerateNextRow(Grid grid, IPseudoRandomNumberGenerator rnd, int y)
            {
                if (grid.IsOutOfBounds(4, y))
                    return false;
                grid.Set(4, y, Grid.PrimitiveType.SmallSegment, Instantiate(TowerGenerator.Instance.PrefabChunkSmall));
                return true;
            }
        }

        public GameObject PrefabChunkSmall;
        private IGeneratorStrategy Generator = new GenericGenerator();
        public Grid _curGrid;
        private int _curRow;
        private IPseudoRandomNumberGenerator _rnd;

        protected override void Awake()
        {
            base.Awake();
            _rnd = RandomHelper.CreateRandomNumberGenerator();
            _curRow = 0;
        }


        void Start()
        {
            var generating = true;
            while (generating)
            {
                generating = Generator.GenerateNextRow(_curGrid, _rnd, _curRow++);
            }
        }
    }
}