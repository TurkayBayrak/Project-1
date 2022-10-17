using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    private GridManager GridManager;
    private Camera cam;
    private Transform camTransform;

    private void Awake()
    {
        if (!instance)
            instance = this;
        cam = Camera.main;
        camTransform = cam.transform;
    }

    private void Start()
    {
        GridManager = GridManager.Instance;
    }
    
    public void SetBounds()
    {
        var maxGridNode = GridManager.transform.GetChild(GridManager.transform.childCount - GridManager.GridMapSize)
            .GetComponent<MeshRenderer>().bounds.max;
        var  minGridNode = GridManager.transform.GetChild(GridManager.GridMapSize - 1)
            .GetComponent<MeshRenderer>().bounds.min;
        
        var objectSizes = maxGridNode - minGridNode;
        const float cameraDistance = 2.0f; // Constant factor
        var objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        var cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * cam.fieldOfView); // Visible height 1 meter in front
        var distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
        distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
        camTransform.position = new Vector3(0,0,0) - distance * camTransform.forward;
    }
}
