using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static readonly string s_kMissingTargetTransformLog = "CameraController is missing the Target Transform component.";
    private static readonly string s_kMissingTransformLog = "CameraController is missing the Transform component.";

    // The axis for the mouse scroll wheel (for camera zooming).
    private static readonly string s_kScrollAxis = "Mouse ScrollWheel";

    [Tooltip("The transform of the current target the camera will follow.")]
    [SerializeField] private Transform m_targetTransform = null;

    [Header("Camera Position")]

    [Tooltip("The offset of the camera from the target.")]
    [SerializeField] private Vector3 m_offsetPosition = new Vector3(0.5f, -0.5f, 0.0f);

    [Tooltip("The offset angle of the camera from the target.")]
    [SerializeField] private float m_offsetAngle = 2.0f;

    [Header("Camera Zoom")]

    [Tooltip("The minimum distance the camera can be zoomed.")]
    [SerializeField] private float m_minZoomDistance = 5.0f;

    [Tooltip("The maximum distance the camera can be zoomed.")]
    [SerializeField] private float m_maxZoomDistance = 20.0f;

    [Tooltip("The speed at which the camera can be zoomed.")]
    [SerializeField] private float m_zoomSpeed = 6.0f;

    // Cache this cameras transform.
    private Transform m_transform = null;

    // The current zoom level of the camera.
    private float m_currentZoomDistance = 10.0f;

    private void Start()
    {
        if (m_transform == null)
            m_transform = transform;
    }

    private void Update()
    {
        // Set the zoom level from the scroll wheel.
        SetCameraZoomLevel();
    }

    private void LateUpdate()
    {
        if (m_targetTransform == null)
        {
            Debug.LogWarning(s_kMissingTargetTransformLog);
            return;
        }
        if (m_transform == null)
        {
            Debug.LogWarning(s_kMissingTransformLog);
            return;
        }

        UpdateCameraTransform();
    }

    // Set the zoom level from the scroll wheel.
    private void SetCameraZoomLevel()
    {
        m_currentZoomDistance -= Input.GetAxis(s_kScrollAxis) * m_zoomSpeed;
        m_currentZoomDistance = Mathf.Clamp(m_currentZoomDistance, m_minZoomDistance, m_maxZoomDistance);
    }

    // Update the camera based on the targets position.
    private void UpdateCameraTransform()
    {
        m_transform.position = m_targetTransform.position - m_offsetPosition * m_currentZoomDistance;
        m_transform.LookAt(m_targetTransform.position + Vector3.up * m_offsetAngle);
    }
}
