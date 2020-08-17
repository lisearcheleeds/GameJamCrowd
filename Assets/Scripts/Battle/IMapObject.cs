using UnityEditor;
using UnityEngine;

public interface IMapObject
{
    void Reset();
    void Apply(MapObjectVO mapObjectVO);
}
