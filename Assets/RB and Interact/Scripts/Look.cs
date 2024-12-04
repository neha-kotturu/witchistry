using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    public float xSensitivity = 30.0f;
    public float ySensitivity = 30.0f;
    private float multiplier = 10.0f;

    private void Start()
    {
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        // Collect and compute input values
        uInput();

        // Apply camera movement
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    // Function that collects and computes input values from raw input
    private void uInput()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity * multiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySensitivity * multiplier;
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}
