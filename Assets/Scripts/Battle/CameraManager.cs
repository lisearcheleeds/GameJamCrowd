using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Vector3 Offset;
    [SerializeField] LayerMask mapLayer;
    [SerializeField] Vector3 effectPosition;

    public Camera MainCamera => mainCamera;
    Transform target;

    public void SetTrackTarget(Transform target)
    {
        mainCamera.transform.position = effectPosition;
        this.target = target;
    }

    public RaycastHit? GetRaycastHit(Ray? ray)
    {
        if (!ray.HasValue)
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        }

        var raycastHit = new RaycastHit();

        if (Physics.Raycast(ray.Value, out raycastHit, 1000.0f, mapLayer))
        {
            return raycastHit;
        }

        return null;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        mainCamera.transform.position += ((target.position + Offset) - mainCamera.transform.position) * 0.2f;
        mainCamera.transform.LookAt(target, Vector3.up);
    }
}