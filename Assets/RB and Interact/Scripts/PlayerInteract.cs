using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public Camera cam;
    public Slider slider;
    [SerializeField] private float distance = 3.0f;
    [SerializeField] private LayerMask mask;
    private PlayerUI playerUI;

    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
    }


    void Update()
    {
        InteractCheck();
    }

    // Function that checks for and handles interaction
    private void InteractCheck()
    {
        playerUI.UpdateText(string.Empty);

        // Cast raycast from center of screen
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;

        // If an object is hit by the ray
        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            // If object is an Interactable
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                // Store object into variable
                Interactable i = hitInfo.collider.GetComponent<Interactable>();

                // Update prompt text
                playerUI.UpdateText(i.promptMessage);

                // if interact button pressed
                if (Input.GetKey(KeyCode.E))
                {
                    // reveal slider, increment value
                    slider.gameObject.SetActive(true);
                    slider.value = Mathf.Clamp(slider.value + 0.003f, slider.minValue, slider.maxValue);

                    // if slider reaches 100%
                    if (slider.value == slider.maxValue)
                    {
                        // hide slider, call interact function
                        slider.gameObject.SetActive(false);
                        i.BaseInteract();
                    }
                }
                else 
                {
                    // hide slider, reset value to 0
                    slider.gameObject.SetActive(false);
                    slider.value = slider.minValue;
                }
            }
        }
        // If raycast no longer detects object (e.g. player looks away before progress bar finished)
        else {
            slider.gameObject.SetActive(false);
            slider.value = slider.minValue;
        }
    }

    // Function that hides all children objects of camera
    public void hideAllItemsOnPlayer()
    {
        for (int i = 0; i < cam.transform.childCount; i++)
        {
            Transform child = cam.transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
    }
}
