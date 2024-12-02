using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation
    public float moveSpeed = 2f; // Speed of movement

    // Update is called once per frame
    void Update()
    {
        // Camera rotation with arrow keys
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime); // Rotate left
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // Rotate right
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime); // Move up
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime); // Move down
        }
    }
}
