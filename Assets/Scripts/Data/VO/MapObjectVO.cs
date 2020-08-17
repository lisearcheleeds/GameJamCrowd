using UnityEditor;
using UnityEngine;

public class MapObjectVO
{
    public string PrefabPath { get; }
    public MapObjectType MapObjectType { get; }

    public Vector3 ObjectPosition { get; }
    public Vector3 ObjectRotate { get; }
    public Vector3 ObjectSize { get; }

    public MapObjectVO(MapObjectType mapObjectType, Vector3 position, Vector3 rotate, Vector3 size)
    {
        MapObjectType = mapObjectType;

        switch (mapObjectType)
        {
            case MapObjectType.Block:
                PrefabPath = "Battle/MapObjectBlock";
                break;
        }

        ObjectPosition = position;
        ObjectRotate = rotate;
        ObjectSize = size;
    }
}
