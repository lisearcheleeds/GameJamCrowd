using UnityEditor;
using UnityEngine;

public class MapVO
{
    public int MapId { get; }
    public int?[] TransitionMapIds { get; }
    public MapObjectVO[] MapObjectVOs { get; }

    public MapVO(int mapId, int?[] transitionMapIds, MapObjectVO[] mapObjectVOs)
    {
        MapId = mapId;
        MapObjectVOs = mapObjectVOs;
        TransitionMapIds = transitionMapIds;
    }
}
