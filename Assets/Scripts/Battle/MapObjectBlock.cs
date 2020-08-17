using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MapObjectBlock : MonoBehaviour, IMapObject
{
    public void Apply(MapObjectVO mapObjectVO)
    {
        gameObject.SetActive(true);

        transform.localPosition = mapObjectVO.ObjectPosition;
        transform.eulerAngles = mapObjectVO.ObjectRotate;
        transform.localScale = mapObjectVO.ObjectSize;
    }

    public void Reset()
    {
        gameObject.SetActive(false);
    }
}