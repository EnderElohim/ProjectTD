using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrCameraController : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float zoomSpeed = 10f;

    private float zoomLevel = 50f;
    private float minZoom = 20f;
    private float maxZoom = 100f;

    private Vector3 startPos;
    private bool isPanning = false;
    private Vector3 panOrigin;
    public float panBorderThickness = 10f;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Move camera with WASD or arrow keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.position += new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;

        // Zoom in/out with mouse scroll wheel
        zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);

        transform.position = new Vector3(transform.position.x, zoomLevel, transform.position.z);

        // Reset camera to starting position with R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = startPos;
            zoomLevel = startPos.y;
        }

        // Panning with right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            isPanning = true;
            panOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - panOrigin);
            Vector3 move = new Vector3(pos.x * moveSpeed * 4 * Time.deltaTime, 0, pos.y * moveSpeed * 4 * Time.deltaTime);
            transform.Translate(-move, Space.World);
        }

        // Move camera when mouse is at edge of screen
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        if (mouseX < panBorderThickness || mouseX >= Screen.width - panBorderThickness ||
            mouseY < panBorderThickness || mouseY >= Screen.height - panBorderThickness)
        {
            Vector3 moveDir = new Vector3((mouseX - Screen.width / 2f) / Screen.width, 0, (mouseY - Screen.height / 2f) / Screen.height);
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
        }
    }
}
