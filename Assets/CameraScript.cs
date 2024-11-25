using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class CameraScript : MonoBehaviour
{
    // Sensitivity and speed settings
    public float mouseSensitivity = 100f;
    public float moveSpeed = 10f;

    private float pitch = 0f; // Vertical rotation (up and down)
    private float yaw = 0f;   // Horizontal rotation (left and right)

    private bool paused = false;
    public GameObject pausePanel;

    public float maxDistance = 10f; // Max interaction distance
    public LayerMask uiLayer; // Layer for UI elements
    public InputActionAsset inputActions; // Assign your InputAction asset

    private InputAction clickAction;
    private PointerEventData pointerEventData;
    private GameObject lastHitObject = null;
    private TMP_Dropdown currentDropdown = null; // To track the currently opened dropdown

    private float lastPauseTime = -100f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Get the click action (adjust based on your InputAction asset)
        clickAction = inputActions.FindAction("Click", true);
        clickAction.Enable();

        // Initialize the PointerEventData
        pointerEventData = new PointerEventData(EventSystem.current);
    }

    // Update is called once per frame
    void Update()
    {

        if (!Input.GetMouseButton((int)UnityEngine.UIElements.MouseButton.RightMouse) && !paused)
        {
            // Mouse input for camera rotation
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Adjust the yaw and pitch
            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -90f, 90f); // Limit the pitch to prevent flipping the camera

            // Apply rotation to the camera
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
        

        // WASD input for movement
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // Space/Shift input for vertical movement (up and down)
        float moveY = 0f;
        if (Input.GetKey(KeyCode.Space)) // Move up
        {
            moveY = 0.25f * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftShift)) // Move down
        {
            moveY = -0.25f * moveSpeed * Time.deltaTime;
        }

        // Apply movement to the camera
        transform.Translate(new Vector3(moveX, moveY, moveZ));


        if (Input.GetKey(KeyCode.Escape) && Time.time >= lastPauseTime + 2f)
        {
            TogglePause();
            lastPauseTime = Time.time;
        }


        if (currentDropdown != null && Input.mouseScrollDelta.y != 0) {
            ScrollRect scrollRect = currentDropdown.transform.Find("Dropdown List")?.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                float scrollDelta = Input.mouseScrollDelta.y * 0.1f * scrollRect.scrollSensitivity;
                float currentScroll = scrollRect.verticalNormalizedPosition;
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(currentScroll + scrollDelta);
            }
        }


        // Create a ray from the camera's forward direction
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Check if the ray hits a UI element
        if (Physics.Raycast(ray, out hit, maxDistance, uiLayer))
        {
            // Set the position of the pointer data to the center of the screen
            pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);

            GameObject hitObject = hit.collider.gameObject;

            // Handle pointerEnter
            if (hitObject != lastHitObject)
            {
                if (lastHitObject != null)
                {
                    // Unhighlight the previous object
                    ExecuteEvents.Execute(lastHitObject, pointerEventData, ExecuteEvents.pointerExitHandler);
                }

                // Highlight the new object
                ExecuteEvents.Execute(hitObject, pointerEventData, ExecuteEvents.pointerEnterHandler);
                lastHitObject = hitObject;
            }

            

            // Handle click action
            if (clickAction.triggered)
            {

                if (hitObject.GetComponent<TMP_Dropdown>() != null)
                {
                    // If clicking the dropdown, open it
                    TMP_Dropdown dropdown = hitObject.GetComponent<TMP_Dropdown>();
                    if (!dropdown.IsExpanded)
                    {
                        // Open the dropdown
                        dropdown.Show();
                        currentDropdown = dropdown;
                    }
                    else
                    {
                        // If it's already open, close it
                        dropdown.Hide();
                        currentDropdown = null;
                    }
                }
                else if (hitObject.GetComponent<Toggle>() != null && currentDropdown != null)
                {
                    // Handle selecting an item from the dropdown (Button component inside the dropdown)
                    Toggle button = hitObject.GetComponent<Toggle>();

                    // Trigger pointer click on the selected item
                    ExecuteEvents.Execute(hitObject, pointerEventData, ExecuteEvents.pointerClickHandler);

                    // Close the dropdown after selection
                    TMP_Dropdown dropdown = currentDropdown.GetComponent<TMP_Dropdown>();
                    dropdown.Hide();
                    currentDropdown = null;
                }
                else if (hitObject.GetComponent<Button>() != null)
                {
                    Button button = hitObject.GetComponent<Button>();
                    ExecuteEvents.Execute(hitObject, pointerEventData, ExecuteEvents.pointerClickHandler);
                }
                else
                {
                    // If clicking outside or on a non-dropdown UI element, close the dropdown
                    if (currentDropdown != null)
                    {
                        TMP_Dropdown dropdown = currentDropdown.GetComponent<TMP_Dropdown>();
                        dropdown.Hide();
                        currentDropdown = null;
                    }
                }
            }
        }
        else
        {
            // If the ray misses, ensure the last highlighted object gets unhighlighted
            if (lastHitObject != null)
            {
                ExecuteEvents.Execute(lastHitObject, pointerEventData, ExecuteEvents.pointerExitHandler);
                lastHitObject = null;
            }

            //if (clickAction.triggered)
            //{
            //    // If clicking outside or on a non-dropdown UI element, close the dropdown
            //    if (currentDropdown != null)
            //    {
            //        TMP_Dropdown dropdown = currentDropdown.GetComponent<TMP_Dropdown>();
            //        dropdown.Hide();
            //        currentDropdown = null;
            //    }
            //}
                
        }
    }


    void TogglePause()
    {
        paused = !paused;

        if (paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(paused);
        }
    }
}
