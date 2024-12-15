using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchOnBoundsExit : MonoBehaviour
{
    // Name of the scene to load when exiting the cabin bounds
    public string sceneToLoad = "SkythianCat/Sunny_Village_LITE/DEMO/Demo_scene";
    public MonoBehaviour cameraMovementScript;  // Reference to the script controlling camera movement

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            // Disable the camera movement while the scene is loading
            if (cameraMovementScript != null)
            {
                cameraMovementScript.enabled = false;
            }

            // Start loading the scene asynchronously
            StartCoroutine(LoadSceneAsync());
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        // Don't allow the scene to be activated until it's fully loaded
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Update progress (you can print or log it, if needed)
            Debug.Log("Loading progress: " + (operation.progress * 100f).ToString("F0") + "%");

            // When loading is almost complete (when progress is near 0.9), activate the scene
            if (operation.progress >= 0.9f)
            {
                // Scene is ready, activate it when any key is pressed
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }

        // Re-enable the camera movement once the scene is loaded
        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = true;
        }
    }
}
