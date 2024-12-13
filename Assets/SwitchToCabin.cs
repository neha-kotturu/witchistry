using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchOnCollision : MonoBehaviour
{
    // Name of the scene to load
    [SerializeField] private string sceneToLoad = "Cabin/Demo";

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision with MainCamera detected. Loading scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected with: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger with MainCamera detected. Loading scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }


}
