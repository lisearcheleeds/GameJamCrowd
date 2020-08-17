using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    [SerializeField] MeshCollider[] transitionAreas;
    [SerializeField] Transform objectParent;
    [SerializeField] LayerMask mapLayerMask;

    public LayerMask MapLayerMask => mapLayerMask;

    MapVO mapVO;

    Dictionary<MapObjectType, GameObject> mapObjectPrefabs = new Dictionary<MapObjectType, GameObject>();
    Dictionary<MapObjectType, List<IMapObject>> mapObjects = new Dictionary<MapObjectType, List<IMapObject>>();
    List<IMapObject> usingObjects = new List<IMapObject>();

    public Vector3? GetTransitionCollider(int mapId)
    {
        for (var i = 0; i < mapVO.TransitionMapIds.Length; i++)
        {
            if (mapVO.TransitionMapIds[i] == mapId)
            {
                return transitionAreas[i].transform.position;
            }
        }

        return null;
    }

    public int? GetTransitionMapId(Collider collider)
    {
        for (var i = 0; i < transitionAreas.Count(); i++)
        {
            if (transitionAreas[i].gameObject == collider.gameObject)
            {
                return mapVO.TransitionMapIds[i];
            }
        }

        return null;
    }

    public void Load(MapVO mapVO)
    {
        this.mapVO = mapVO;

        foreach (var usingObject in usingObjects)
        {
            usingObject.Reset();
        }

        usingObjects.Clear();

        foreach (var mapObjectVO in mapVO.MapObjectVOs)
        {
            if (!mapObjects.ContainsKey(mapObjectVO.MapObjectType))
            {
                mapObjects[mapObjectVO.MapObjectType] = new List<IMapObject>();
            }

            if (!mapObjectPrefabs.ContainsKey(mapObjectVO.MapObjectType))
            {
                mapObjectPrefabs[mapObjectVO.MapObjectType] = Resources.Load<GameObject>(mapObjectVO.PrefabPath);
            }

            while (mapObjects[mapObjectVO.MapObjectType].Count < mapVO.MapObjectVOs.Count(vo => vo.MapObjectType == mapObjectVO.MapObjectType))
            {
                mapObjects[mapObjectVO.MapObjectType].Add(Instantiate(mapObjectPrefabs[mapObjectVO.MapObjectType], objectParent).GetComponent<IMapObject>());
            }

            for (var i = 0; i < mapObjects[mapObjectVO.MapObjectType].Count; i++)
            {
                if (!usingObjects.Contains(mapObjects[mapObjectVO.MapObjectType][i]))
                {
                    usingObjects.Add(mapObjects[mapObjectVO.MapObjectType][i]);
                    mapObjects[mapObjectVO.MapObjectType][i].Apply(mapObjectVO);
                    break;
                }
            }
        }
    }
}
