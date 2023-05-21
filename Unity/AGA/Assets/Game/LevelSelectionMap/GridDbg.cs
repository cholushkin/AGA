using System.Collections;
using System.Collections.Generic;
using GameLib;
using NaughtyAttributes;
using UnityEngine;

public partial class Grid : MonoBehaviour
{
    [HorizontalLine(8f, EColor.Gray)]
    [Header("Debug")]
    public float DbgDrawDepth;
    public int DbgX;
    public int DbgY;
    public int DbgExtX;
    public int DbgExtY;
    public PrimitiveType Primitive;
    [Button()]
    void Set()
    {
        if (DbgExtX < 1 || DbgExtY < 1)
        {
            Debug.LogError("Should be DbgExtX < 1 || DbgExtY < 1");
            return;
        }

        Set(DbgX, DbgY, Primitive, null, DbgExtX, DbgExtY);
    }

    [Button()]
    void Delete()
    {
        Delete(DbgX, DbgY);
    }

    [Button()]
    void Get()
    {
        var cell = _cells[DbgX, DbgY] ?? Grid.EmptyCell;
        Debug.Log($"{DbgX}:{DbgY}: {cell.Primitive} PartialCell:{cell is PartialCell} ParentCell:{cell is ParentCell}");
    }

    [Button()]
    void Print()
    {
        for (int x = 0; x < GridSize.x; ++x)
        for (int y = 0; y < GridSize.y; ++y)
        {
            var cell = _cells[x, y] ?? Grid.EmptyCell;
            Debug.Log($"{x}:{y}: {cell.Primitive} PartialCell:{cell is PartialCell} ParentCell:{cell is ParentCell}");
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        var cellSize3D = new Vector3(CellSize.x, CellSize.y, DbgDrawDepth);
        var gridSize3D = new Vector3(GridSize.x, GridSize.y, 1);

        // Draw grid
        Gizmos.color = Color.green;
        var size = new Vector3(GridSize.x * CellSize.x, GridSize.y * CellSize.y, DbgDrawDepth);
        Gizmos.DrawWireCube(new Vector3(size.x * 0.5f, size.y * 0.5f, 0f), Vector3.Scale(gridSize3D, cellSize3D));

        for (int x = 0; x < GridSize.x; ++x)
            Gizmos.DrawLine(
                new Vector3(x * CellSize.x, 0f, 0f),
                new Vector3(x * CellSize.x, size.y, 0f));
        for (int y = 0; y < GridSize.y; ++y)
            Gizmos.DrawLine(
                new Vector3(0f, y * CellSize.y, 0f),
                new Vector3(size.x, y * CellSize.y, 0f));

        // Draw cells
        if (_cells != null)
        {
            for (int x = 0; x < GridSize.x; ++x)
                for (int y = 0; y < GridSize.y; ++y)
                {
                    var cell = _cells[x, y];
                    if (cell != null)
                    {
                        var pos = new Vector3(x * cellSize3D.x, y * cellSize3D.y, 0) + new Vector3(cellSize3D.x * 0.5f, cellSize3D.y * 0.5f, 0f);
                        if (cell.Primitive == PrimitiveType.SmallSegment)
                        {
                            Gizmos.color = new Color(0, 1f, 0, 0.3f);
                            Gizmos.DrawCube(pos, cellSize3D);
                        }
                    }
                }
        }
    }
}
