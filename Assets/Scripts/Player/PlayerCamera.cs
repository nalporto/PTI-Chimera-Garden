using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private Transform weapon; // Assign in Inspector
    [SerializeField] private Vector3 weaponOffset = new Vector3(0f, -0.2f, 2.0f);
    private Vector3 _eulerAngles;
    [SerializeField] private Camera mainCamera; // Drag your Camera here in the Inspector
    private float _currentFov;

    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.eulerAngles = _eulerAngles = target.eulerAngles;
        if (weapon != null)
        {
            weapon.position = transform.position + transform.TransformDirection(weaponOffset);
            weapon.rotation = transform.rotation * Quaternion.Euler(0,0,0);
        }
    }

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
            Debug.LogError("No Camera component found on " + gameObject.name);
        _currentFov = mainCamera != null ? mainCamera.fieldOfView : 60f;

        // Ensure weapon is rotated at play
        if (weapon != null)
        {
            weapon.rotation = transform.rotation * Quaternion.Euler(0,0,0);
        }
    }

    public void UpdateRotation(CameraInput input)
    {
        _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * sensitivity;
        transform.eulerAngles = _eulerAngles;
        if (weapon != null)
        {
            weapon.position = transform.position + transform.TransformDirection(weaponOffset);
            weapon.rotation = transform.rotation;
        }
    }

    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
        if (weapon != null)
        {
            weapon.position = transform.position + transform.TransformDirection(weaponOffset);
            weapon.rotation = transform.rotation;
        }
    }

    public void UpdateFov(float targetFov, float lerpSpeed = 10f)
    {
        if (mainCamera == null) return;
        _currentFov = Mathf.Lerp(_currentFov, targetFov, Time.deltaTime * lerpSpeed);
        mainCamera.fieldOfView = _currentFov;
    }
}
