using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;
    [SerializeField] private bool lockZ = false;

    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main?.transform;

        if (cameraTransform == null)
        {
            Debug.LogWarning("Billboard: Main camera not found!");
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 direction = cameraTransform.position - transform.position;

        if (lockX) direction.x = 0;
        if (lockY) direction.y = 0;
        if (lockZ) direction.z = 0;

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(-direction);
            transform.rotation = rotation;
        }
    }
}