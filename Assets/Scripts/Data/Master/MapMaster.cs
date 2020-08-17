using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MapMaster
{
    public static MapVO GetMapVO(int id, (int row, int column)[] layouts, int cellSideCount)
    {
        return new MapVO(
            id,
            GetTransitionMapIds(id, layouts, cellSideCount),
            GetMapObjectVOs(id));
    }

    public static int?[] GetTransitionMapIds(int id, (int row, int column)[] layouts, int cellSideCount)
    {
        var mapPosition = layouts[id];

        var columnCountMax = cellSideCount * 2 - 1;

        var columnCount = columnCountMax - Mathf.Abs(mapPosition.row - columnCountMax / 2);
        var prevColumnCount = columnCountMax - Mathf.Abs(mapPosition.row - 1 - columnCountMax / 2);
        var nextColumnCount = columnCountMax - Mathf.Abs(mapPosition.row + 1 - columnCountMax / 2);

        var prevOffset = Mathf.Max(prevColumnCount, columnCount);
        var nextOffset = Mathf.Min(nextColumnCount, columnCount);

        //  1 2
        // 0   3
        //  5 4
        // の順で格納する
        return new int?[]
        {
            GetLayout(layouts, id - 1, mapPosition.row),
            GetLayout(layouts, id - prevOffset, mapPosition.row - 1),
            GetLayout(layouts, id - prevOffset + 1, mapPosition.row - 1),
            GetLayout(layouts, id + 1, mapPosition.row),
            GetLayout(layouts, id + nextOffset + 1, mapPosition.row + 1),
            GetLayout(layouts, id + nextOffset, mapPosition.row + 1),
        };
    }

    static MapObjectVO[] GetMapObjectVOs(int id)
    {
        var res = new List<MapObjectVO>();
        if (id % 2 == 0)
        {
            res.AddRange(new[]
            {
                new MapObjectVO(MapObjectType.Block, Vector3.zero, Vector3.zero, new Vector3(10.0f, 0.5f, 40.0f)),
                new MapObjectVO(MapObjectType.Block, Vector3.zero, Vector3.zero, new Vector3(8.0f, 1.5f, 38.0f)),
                new MapObjectVO(MapObjectType.Block, Vector3.zero, Vector3.zero, new Vector3(6.0f, 2.5f, 36.0f)),
                new MapObjectVO(MapObjectType.Block, Vector3.zero, Vector3.zero, new Vector3(4.0f, 3.5f, 34.0f)),
                new MapObjectVO(MapObjectType.Block, Vector3.zero, Vector3.zero, new Vector3(2.0f, 4.5f, 32.0f)),
            });
        }

        if (id % 3 == 0)
        {
            res.Add(new MapObjectVO(MapObjectType.Block, Vector3.zero, Vector3.left * 45, Vector3.one * 10));
        }

        if (id % 3 == 1)
        {
            res.AddRange(new[]
            {
                new MapObjectVO(MapObjectType.Block, new Vector3(15, 0, 15), Vector3.zero, new Vector3(4.0f, 8.0f, 4.0f)),
                new MapObjectVO(MapObjectType.Block, new Vector3(15, 0, -15), Vector3.zero, new Vector3(4.0f, 8.0f, 4.0f)),
                new MapObjectVO(MapObjectType.Block, new Vector3(-15, 0, 15), Vector3.zero, new Vector3(4.0f, 8.0f, 4.0f)),
                new MapObjectVO(MapObjectType.Block, new Vector3(-15, 0, -15), Vector3.zero, new Vector3(4.0f, 8.0f, 4.0f)),
            });
        }

        return res.ToArray();
    }

    static int? GetLayout((int row, int column)[] layouts, int index, int row)
    {
        if (index < 0 || layouts.Length <= index)
        {
            return null;
        }

        if (layouts[index].row != row)
        {
            return null;
        }

        return index;
    }
}
