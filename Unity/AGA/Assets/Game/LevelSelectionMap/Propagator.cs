using System.Collections.Generic;
using GameLib;
using GameLib.Alg;
using UnityEngine;

namespace Core
{
    public class Propagator : Singleton<Propagator>
    {
        private HashSet<Grid> _grids = new HashSet<Grid>(8);
        // Propagate to coordinate space of target (x,y) grid
        // x, y - coordinates in fromGrid coordinate system
        // If x,y are outside fromGrid returns neighbor-grid and new converted x,y in that grid 
        // If x,y are inside fromGrid returns passed x, y and grid
        public (Grid grid, int x, int y, Grid.Cell cell) Propagate(Grid fromGrid, int x, int y)
        {
            bool outOfGrid = (x < 0) || (y < 0) || (x >= fromGrid.GridSize.x) || (y >= fromGrid.GridSize.y);
            if (!outOfGrid)
            {
                var cell = fromGrid._cells[x, y] ?? Grid.EmptyCell;
                return (fromGrid, x, y, cell);
            }

            // Get grid
            var targetPos = 
                    fromGrid.transform.position.ToVector2() + 
                    Vector2.Scale(new Vector2(x, y), Grid.CellSize) +
                    new Vector2(Grid.CellSize.x, Grid.CellSize.y) * 0.5f; // target point in world space
           var targetGrid = GetGrid(fromGrid, targetPos);

           if (targetGrid.grid == null)
                return (null, 0, 0, null);

            // Get cell inside the grid
            var delta = targetPos - targetGrid.gridPos;
            int resX = (int)Mathf.Floor(delta.x / Grid.CellSize.x);
            int resY = (int)Mathf.Floor(delta.y / Grid.CellSize.y);

            var tCell = targetGrid.grid._cells[resX, resY] ?? Grid.EmptyCell;
            return (targetGrid.grid, resX, resY, tCell);
        }

        // Propagate to toGrid coordinate space
        // x, y - coordinates in fromGrid coordinate system
        // toGrid - is a grid in which coordinate system we need to do converting
        public (Grid grid, int x, int y, Grid.Cell cell) Propagate(Grid fromGrid, int x, int y, Grid toGrid)
        {
            // Get grid
            var targetPos =
                fromGrid.transform.position.ToVector2() +
                Vector2.Scale(new Vector2(x, y), Grid.CellSize) +
                new Vector2(Grid.CellSize.x, Grid.CellSize.y) * 0.5f; // target point in world space
            var targetGrid = (grid:toGrid, gridPos:toGrid.transform.position.ToVector2());

            // Get cell inside the grid
            var delta = targetPos - targetGrid.gridPos;
            int resX = (int)Mathf.Floor(delta.x / Grid.CellSize.x);
            int resY = (int)Mathf.Floor(delta.y / Grid.CellSize.y);

            var tCell = Propagate(toGrid, resX, resY).cell;
            return (targetGrid.grid, resX, resY, tCell);
        }

        public void RegisterGrid(Grid grid)
        {
            _grids.Add(grid);
        }

        private (Grid grid, Vector2 gridPos) GetGrid(Grid fromGrid, Vector2 wPos)
        {
            Vector2 targetGridPos = Vector2.zero;
            Grid targetGrid = null;
            foreach (var grid in _grids)
            {
                var gridWorldSize = Vector2.Scale(grid.GridSize, Grid.CellSize);
                var gridPos = grid.transform.position.ToVector2();
                if (wPos.x < gridPos.x)
                    continue;
                if (wPos.x > gridPos.x + gridWorldSize.x)
                    continue;
                if (wPos.y < gridPos.y)
                    continue;
                if (wPos.y > gridPos.y + gridWorldSize.y)
                    continue;

                targetGrid = grid;
                targetGridPos = gridPos;
                break;
            }

            return (targetGrid, targetGridPos);
        }
    }
}
