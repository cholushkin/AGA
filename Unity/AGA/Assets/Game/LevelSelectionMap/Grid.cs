using Core;
using UnityEngine;
using UnityEngine.Assertions;

public partial class Grid : MonoBehaviour
{


    public enum PrimitiveType
    {
        None,
        SmallSegment,
        MediumSegment,
        CustomSegment, // the one which replaces MediumSegments by pattern matching
        Roof,
        Decoration,
        DisabledCell
    }

    public class Cell
    {
        public GameObject GameObject { get; internal set; }
        public PrimitiveType Primitive { get; internal set; }
    }

    public class ParentCell : Cell
    {
        public Vector2Int[] Parts { get; internal set; }
    }

    public class PartialCell : Cell
    {
        public Vector2Int Parent { get; internal set; }
    }



    public static readonly Cell EmptyCell = new() { Primitive = PrimitiveType.None };
    public Propagator Propagator;
    public Vector2Int GridSize;
    public static readonly Vector2Int CellSize = Vector2Int.one;
    internal Cell[,] _cells;


    public void Awake()
    {
        Propagator.RegisterGrid(this);
        _cells = new Cell[GridSize.x, GridSize.y];
    }


    // Delete cell index coordinates
    // If the cell on x,y index is partial cell then entire block will be deleted (parent and all parts)
    // Takes in account that the cell might be out of the current grid on a neighbor grid
    public void Delete(int x, int y)
    {
        if (_cells == null)
            return;
        var targetRef = Propagator.Propagate(this, x, y);

        // Trying to delete empty cell
        if (targetRef.cell.Primitive == PrimitiveType.None)
            return;

        (Grid grid, int x, int y, Cell cell)? parentCellRef = null;
        (Grid grid, int x, int y, Cell cell)? deleteCellRef = null;

        // Assign references
        if (targetRef.cell is PartialCell partialCell)
        {
            parentCellRef = Propagator.Propagate(targetRef.grid, partialCell.Parent.x, partialCell.Parent.y);
            deleteCellRef = parentCellRef;
        }
        else if (targetRef.cell is ParentCell)
        {
            parentCellRef = targetRef;
            deleteCellRef = parentCellRef;
        }
        else // Cell
        {
            deleteCellRef = targetRef;
        }

        // Delete parts
        if (parentCellRef.HasValue)
        {
            var parentCell = parentCellRef.Value.cell as ParentCell;

            // Set all partial cells ref to null
            foreach (var partialCellIndex in parentCell.Parts)
            {
                var partCellRef = Propagator.Propagate(parentCellRef.Value.grid, partialCellIndex.x, partialCellIndex.y);
                partCellRef.grid._cells[partCellRef.x, partCellRef.y] = null;
            }
        }

        // Delete root
        if (deleteCellRef.Value.cell.GameObject != null)
            Destroy(deleteCellRef.Value.cell.GameObject);
        deleteCellRef.Value.grid._cells[deleteCellRef.Value.x, deleteCellRef.Value.y] = null;
    }

    public void Set(int x, int y, PrimitiveType primitive, GameObject gObject, int extendX = 1, int extendY = 1)
    {
        Assert.IsTrue(primitive != PrimitiveType.None, "Use Delete instead");
        if (_cells == null)
            return;

        // Delete prev values
        for (int ix = x; ix < x + extendX; ix++)
            for (int iy = y; iy < y + extendY; iy++)
            {
                var cellRef = Propagator.Propagate(this, ix, iy);
                if (cellRef.grid == null)
                {
                    Debug.LogError($"{this}:{ix}:{iy} no grid propagation");
                    return;
                }
                cellRef.grid.Delete(cellRef.x, cellRef.y);
            }

        // Set root Cell 
        if (extendX == 1 && extendY == 1)
        {
            var targetCellRef = Propagator.Propagate(this, x, y);
            Assert.IsTrue(targetCellRef.cell.Primitive == PrimitiveType.None);
            var rootCell = new Cell
            {
                GameObject = gObject,
                Primitive = primitive
            };
            if (gObject != null)
            {
                gObject.transform.SetParent(targetCellRef.grid.transform);
                gObject.transform.position = targetCellRef.grid.transform.position + new Vector3(targetCellRef.x * CellSize.x, targetCellRef.y * CellSize.y, 0);
            }

            targetCellRef.grid._cells[targetCellRef.x, targetCellRef.y] = rootCell;
        }
        // Set root ParentCell & PartialCells
        else
        {
            (Grid grid, int x, int y, Cell cell)? parentCellRef = null;
            int partialIndex = 0;
            for (int ix = x; ix < x + extendX; ix++)
                for (int iy = y; iy < y + extendY; iy++)
                {
                    if (ix == x && iy == y)
                    {
                        parentCellRef = Propagator.Propagate(this, ix, iy);
                        Assert.IsTrue(parentCellRef.Value.cell.Primitive == PrimitiveType.None);

                        var parentCell = new ParentCell
                        {
                            GameObject = gObject,
                            Primitive = primitive,
                            Parts = new Vector2Int[extendX * extendY - 1]
                        };
                        if (gObject != null)
                        {
                            gObject.transform.SetParent(parentCellRef.Value.grid.transform);
                            gObject.transform.position = parentCellRef.Value.grid.transform.position 
                                + new Vector3(parentCellRef.Value.x * CellSize.x, parentCellRef.Value.y * CellSize.y, 0);
                        }

                        parentCellRef.Value.grid._cells[parentCellRef.Value.x, parentCellRef.Value.y] = parentCell;

                        parentCellRef = Propagator.Propagate(this, ix, iy);
                        Assert.IsTrue(parentCellRef.Value.cell.Primitive == primitive);

                    }
                    else
                    {
                        var targetCellRef = Propagator.Propagate(this, ix, iy);
                        Assert.IsTrue(targetCellRef.cell.Primitive == PrimitiveType.None);

                        var parentCellInPartialCellSpaceRef = Propagator.Propagate(
                            parentCellRef.Value.grid,
                            parentCellRef.Value.x, parentCellRef.Value.y,
                            targetCellRef.grid);

                        var partialCell = new PartialCell()
                        {
                            GameObject = null,
                            Primitive = primitive,
                            Parent = new Vector2Int(parentCellInPartialCellSpaceRef.x, parentCellInPartialCellSpaceRef.y)
                        };
                        targetCellRef.grid._cells[targetCellRef.x, targetCellRef.y] = partialCell;
                        var parentCell = parentCellRef.Value.cell as ParentCell;

                        var partCellInParentSpaceRef =
                            Propagator.Propagate(targetCellRef.grid, targetCellRef.x, targetCellRef.y, parentCellRef.Value.grid);
                        parentCell.Parts[partialIndex++] = new Vector2Int(partCellInParentSpaceRef.x, partCellInParentSpaceRef.y);
                    }
                }
        }
    }

    public Cell Get(int x, int y)
    {
        if (_cells == null)
            return EmptyCell;
        return Propagator.Propagate(this, x, y).cell;
    }

    public bool IsOutOfBounds(int x, int y)
    {
        return Propagator.Propagate(this, x, y).grid == null;
    }
}