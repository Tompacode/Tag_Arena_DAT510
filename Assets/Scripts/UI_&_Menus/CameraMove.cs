using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Camera targetCamera;

    [Header("Follow")]
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 2f, -10f);
    [SerializeField] private float followSmoothTime = 0.15f;

    [Header("X Limits")]
    [SerializeField] private float minX = -20f;
    [SerializeField] private float maxX = 20f;

    [Header("Zoom (Perspective FOV)")]
    [SerializeField] private float minFov = 50f;      // zoom in limit
    [SerializeField] private float maxFov = 80f;      // zoom out limit
    [SerializeField] private float zoomMultiplier = 0.5f;
    [SerializeField] private float zoomSmoothSpeed = 6f;

    private Vector3 followVelocity;

    private void Awake()
    {
        if (playerManager == null)
        {
            playerManager = PlayerManager.Instance;
        }

        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }
    }

    private void LateUpdate()
    {
        if (playerManager == null || targetCamera == null)
        {
            return;
        }

        GameObject p1 = playerManager.GetActivePlayer1();
        GameObject p2 = playerManager.GetActivePlayer2();

        if (p1 == null && p2 == null)
        {
            return;
        }

        Vector3 midpoint = GetMidpoint(p1, p2);
        Vector3 desiredPosition = midpoint + followOffset;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, followSmoothTime);

        UpdateZoom(p1, p2);
    }

    private Vector3 GetMidpoint(GameObject p1, GameObject p2)
    {
        if (p1 != null && p2 != null)
        {
            return (p1.transform.position + p2.transform.position) * 0.5f;
        }

        return p1 != null ? p1.transform.position : p2.transform.position;
    }

    private void UpdateZoom(GameObject p1, GameObject p2)
    {
        float distanceX = 0f;

        if (p1 != null && p2 != null)
        {
            distanceX = Mathf.Abs(p1.transform.position.x - p2.transform.position.x);
        }

        float targetFov = Mathf.Clamp(minFov + (distanceX * zoomMultiplier), minFov, maxFov);

        if (!targetCamera.orthographic)
        {
            targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, targetFov, Time.deltaTime * zoomSmoothSpeed);
        }
    }
}
