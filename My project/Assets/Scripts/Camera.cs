using UnityEngine;

public class Camera_pan : MonoBehaviour
{
    public float panSpeed = 6f;
    public Transform player; 
    public Vector3 offset = new Vector3(0f, 15f, -1f); // Adjusted for top-down view (Y-axis height)
    public float minHeight = 5f; // Minimum camera height to prevent clipping
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        // Ensure near clipping plane is appropriate for camera height
        _camera.nearClipPlane = -30f; // Prevents nearby clipping
    }

    void Update()
    {
        HandlePanInput();
        FollowPlayer();
    }

    void HandlePanInput()
    {
        Vector2 panPosition = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 moveDelta = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0)
                          * new Vector3(panPosition.x, 0, panPosition.y)
                          * (panSpeed * Time.deltaTime);

        transform.position += moveDelta;
        // Maintain minimum height after panning
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Max(transform.position.y, minHeight),
            transform.position.z
        );
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            Vector3 desiredPos = player.position + offset;
            desiredPos.y = Mathf.Max(desiredPos.y, minHeight);
            transform.position = Vector3.Lerp(transform.position, desiredPos, panSpeed * Time.deltaTime);
        }
    }
}